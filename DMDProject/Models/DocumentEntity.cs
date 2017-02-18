using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DMDProject.Models
{
    public class DocumentEntity
    {
        public string Title { get; set; }
        public string Document_type { get; set; }
        public string Year { get; set; }
        public string Source_id { get; set; }
        public string Volume { get; set; }
        public string Url { get; set; }
        public string Ee { get; set; }
        public string Keywords { get; set; }
        public string Document_id { get; set; }

    }

    public interface IDocumentEntityProvider
    {
        IList<DocumentEntity> GetAllEntities();
        void Insert(DocumentEntity document);
        void Delete(string Document_id);
        IList<DocumentEntity> Find(string Document_id);
        void Edit(DocumentEntity document);
        IList<AuthorEntity> Details(string id);
        IList<DocumentEntity> Search(string Title, string Document_type, string Year, string Keyword);
    }

    public class DefaultPostgresProviderDocument : IDocumentEntityProvider
    {
        BPlusTree<int, int>.OptionsV2 options = new BPlusTree<int, int>.OptionsV2(PrimitiveSerializer.Int32, PrimitiveSerializer.Int32);
       
        public IList<DocumentEntity> GetAllEntities()
        {
            var list = new List<DocumentEntity>();
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.Never;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeDocument");
            int count = 0;
            using (var tree = new BPlusTree<int, int>(options))
            {
                using (FileStream fs = new FileStream("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/Document", FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        foreach (var item in tree)
                        {
                            if (count == 50)
                                return list;
                            fs.Seek(item.Value, 0);
                            list.Add(new DocumentEntity()
                            {
                                Document_id = br.ReadInt32().ToString(),
                                Title = br.ReadString(),
                                Document_type = br.ReadString(),
                                Year = br.ReadInt32().ToString(),
                                Volume = br.ReadString(),
                                Url = br.ReadString(),
                                Ee = br.ReadString(),
                                Keywords = br.ReadString(),
                                Source_id = br.ReadInt32().ToString()
                            });
                            count++;
                        }
                    }
                }
            }
            return list;            
        }

        public void Insert(DocumentEntity document)
        {
            if (document.Title == null)
                throw new ArgumentNullException();
            if (document.Document_type == null)
                document.Document_type = " ";
            if (document.Year == null)
                document.Year = "0";
            foreach(var item in document.Year)
            {
                if (!Char.IsNumber(item))
                    throw new FormatException();
            }
            if (document.Volume == null)
                document.Volume = " ";
            if (document.Url == null)
                document.Url = " ";
            if (document.Ee == null)
                document.Ee = " ";
            if (document.Keywords == null)
                document.Keywords = " ";
            if (document.Source_id == null)
                document.Source_id = "0";
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.Never;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeDocument");
            using (FileStream fs = new FileStream("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/Document", FileMode.OpenOrCreate))
            {
                using (var tree = new BPlusTree<int, int>(options))
                {
                    KeyValuePair<int, int> kv = tree.Last();
                    int key = kv.Key;
                    key++;
                    document.Document_id = key.ToString();
                    tree.Add(Convert.ToInt32(document.Document_id), (int)fs.Length);
                }
                fs.Seek((int)fs.Length, 0);

                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(Convert.ToInt32(document.Document_id));
                    bw.Write(document.Title);
                    bw.Write(document.Document_type);
                    bw.Write(Convert.ToInt32(document.Year));
                    bw.Write(document.Volume);
                    bw.Write(document.Url);
                    bw.Write(document.Ee);
                    bw.Write(document.Keywords);
                    bw.Write(Convert.ToInt32(document.Source_id));
                }
            }
        }

        public void Delete(string Document_id)
        {
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.Never;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeDocument");
            using (var tree = new BPlusTree<int, int>(options))
            {
                tree.Remove(Convert.ToInt32(Document_id));
            }
        }

        public IList<DocumentEntity> Find(string id)
        {
            var list = new List<DocumentEntity>();
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.IfNeeded;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeDocument");
            KeyValuePair<int, int> a = new KeyValuePair<int, int>();
            using (var tree = new BPlusTree<int, int>(options))
            {
                foreach (var item in tree)
                {
                    if (item.Key == Convert.ToInt32(id))
                        a = item;
                }
            }
            using (FileStream fs = new FileStream("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/Document", FileMode.Open))
            {
                fs.Seek(a.Value, 0);
                using (BinaryReader br = new BinaryReader(fs))
                {
                    list.Add(new DocumentEntity()
                    {
                        Document_id = br.ReadInt32().ToString(),
                        Title = br.ReadString(),
                        Document_type = br.ReadString(),
                        Year = br.ReadInt32().ToString(),
                        Volume = br.ReadString(),
                        Url = br.ReadString(),
                        Ee = br.ReadString(),
                        Keywords = br.ReadString(),
                        Source_id = br.ReadInt32().ToString()
                    });
                }
            }
            return list;         
        }

        public void Edit(DocumentEntity document)
        {
            if (document.Title == null)
                throw new ArgumentNullException();
            if (document.Document_type == null)
                document.Document_type = " ";
            if (document.Year == null)
                document.Year = "0";
            foreach (var item in document.Year)
            {
                if (!Char.IsNumber(item))
                    throw new FormatException();
            }
            if (document.Volume == null)
                document.Volume = " ";
            if (document.Url == null)
                document.Url = " ";
            if (document.Ee == null)
                document.Ee = " ";
            if (document.Keywords == null)
                document.Keywords = " ";
            if (document.Source_id == null)
                document.Source_id = "0";
            Delete(document.Document_id);

            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.Never;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeDocument");
            using (FileStream fs = new FileStream("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/Document", FileMode.OpenOrCreate))
            {
                using (var tree = new BPlusTree<int, int>(options))
                {
                    tree.Add(Convert.ToInt32(document.Document_id), (int)fs.Length);
                }
                fs.Seek((int)fs.Length, 0);

                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(Convert.ToInt32(document.Document_id));
                    bw.Write(document.Title);
                    bw.Write(document.Document_type);
                    bw.Write(Convert.ToInt32(document.Year));
                    bw.Write(document.Volume);
                    bw.Write(document.Url);
                    bw.Write(document.Ee);
                    bw.Write(document.Keywords);
                    bw.Write(Convert.ToInt32(document.Source_id));
                }
            }
        }

        public IList<Author_DocumentEntity> FindDocument(string id)
        {
            var list = new List<Author_DocumentEntity>();
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.IfNeeded;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeAuthor_Document");
            using (var tree = new BPlusTree<int, int>(options))
            {
                using (FileStream fs = new FileStream("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/Author_Document", FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        foreach (var item in tree)
                        {
                            fs.Seek(item.Value, 0);

                            string A_D_id = br.ReadInt32().ToString();
                            string Author_id = br.ReadInt32().ToString();
                            string Document_id = br.ReadInt32().ToString();
                            if (Document_id == id)
                            {
                                list.Add(new Author_DocumentEntity()
                                {
                                    A_D_id = A_D_id,
                                    Author_id = Author_id,
                                    Document_id = Document_id
                                });
                            }
                        }
                    }
                }
            }
            return list;
        }

        public IList<AuthorEntity> Details(string id)
        {
            DefaultPostgresProviderAuthor author = new DefaultPostgresProviderAuthor();
            var author_document = FindDocument(id);
            List<AuthorEntity> a = new List<AuthorEntity>();
            foreach (var item in author_document)
            {
                var d = author.Find(item.Author_id.ToString());
                a.Add(d.First());
            }
            IList<AuthorEntity> auth = a;
            return auth;
        }

        public IList<DocumentEntity> Search(string title, string document_type, string year, string keyword)
        {
            if (title == null)
                title = "";
            if (year == null)
                year = "";
            if (document_type == null)
                document_type = "";
            if (keyword == null)
                keyword = "";
            var list = new List<DocumentEntity>();
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.Never;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeDocument");
            using (var tree = new BPlusTree<int, int>(options))
            {
                using (FileStream fs = new FileStream("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/Document", FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        foreach (var item in tree)
                        {
                            fs.Seek(item.Value, 0);
                            string Document_id = br.ReadInt32().ToString();
                            string Title = br.ReadString();
                            string Document_type = br.ReadString();
                            string Year = br.ReadInt32().ToString();
                            string Volume = br.ReadString();
                            string Url = br.ReadString();
                            string Ee = br.ReadString();
                            string Keywords = br.ReadString();
                            string Source_id = br.ReadInt32().ToString();

                            if (Title.Contains(title))
                                if (document_type.Contains(document_type))
                                    if (Year.Contains(year))
                                        if (Keywords.Contains(keyword))
                                            list.Add(new DocumentEntity()
                                            {
                                                Document_id = Document_id,
                                                Title = Title,
                                                Document_type = Document_type,
                                                Year = Year,
                                                Volume = Volume,
                                                Url = Url,
                                                Ee = Ee,
                                                Keywords = Keywords,
                                                Source_id = Source_id
                                            });
                        }
                    }
                }
            }
            return list;
        }

    }
}