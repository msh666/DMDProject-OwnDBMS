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
    public class Author_DocumentEntity
    {
        public string A_D_id { get; set; }
        public string Author_id { get; set; }
        public string Document_id { get; set; }
    }

    public interface IAuthor_DocumentEntityProvider
    {
        IList<Author_DocumentEntity> GetAllEntities();
        void Insert(Author_DocumentEntity a_d);
        void Delete(string a_d_id);
        IList<Author_DocumentEntity> Find(string id);
        void Edit(Author_DocumentEntity a_d);
        Dictionary<string, string> Details(string a_d_id);
    }

    public class DefaultPostgresProviderAuthor_Document : IAuthor_DocumentEntityProvider
    {
        BPlusTree<int, int>.OptionsV2 options = new BPlusTree<int, int>.OptionsV2(PrimitiveSerializer.Int32, PrimitiveSerializer.Int32);

       
        public IList<Author_DocumentEntity> GetAllEntities()
        {
            var list = new List<Author_DocumentEntity>();
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.Never;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeAuthor_Document");
            int count = 0;
            using (var tree = new BPlusTree<int, int>(options))
            {
                using (FileStream fs = new FileStream("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/Author_Document", FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        foreach (var item in tree)
                        {
                            if (count == 50)
                                return list;
                            fs.Seek(item.Value, 0);
                            list.Add(new Author_DocumentEntity()
                            {
                                A_D_id = br.ReadInt32().ToString(),
                                Author_id = br.ReadInt32().ToString(),
                                Document_id = br.ReadInt32().ToString()
                            });
                            count++;
                        }
                    }
                }
            }
            return list;
        }

        public void Insert(Author_DocumentEntity a_d)
        {
            if (a_d.Author_id == null)
                throw new ArgumentNullException();
            foreach (var item in a_d.Author_id)
            {
                if (!Char.IsNumber(item))
                    throw new FormatException();
            }
            if (a_d.Document_id == null)
                throw new ArgumentNullException();
            foreach (var item in a_d.Document_id)
            {
                if (!Char.IsNumber(item))
                    throw new FormatException();
            }
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.Never;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeAuthor_Document");
            using (FileStream fs = new FileStream("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/Author_Document", FileMode.OpenOrCreate))
            {
                using (var tree = new BPlusTree<int, int>(options))
                {
                    KeyValuePair<int, int> kv = tree.Last();
                    int key = kv.Key;
                    key++;
                    a_d.A_D_id = key.ToString();
                    tree.Add(Convert.ToInt32(a_d.A_D_id), (int)fs.Length);
                }
                fs.Seek((int)fs.Length, 0);

                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(Convert.ToInt32(a_d.A_D_id));
                    bw.Write(Convert.ToInt32(a_d.Author_id));
                    bw.Write(Convert.ToInt32(a_d.Document_id));
                }
            }
        }

        public void Delete(string a_d_id)
        {
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.Never;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeAuthor_Document");
            using (var tree = new BPlusTree<int, int>(options))
            {
                tree.Remove(Convert.ToInt32(a_d_id));
            }
        }

        public IList<Author_DocumentEntity> Find(string id)
        {
            var list = new List<Author_DocumentEntity>();
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.IfNeeded;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeAuthor_Document");
            KeyValuePair<int, int> a = new KeyValuePair<int, int>();
            using (var tree = new BPlusTree<int, int>(options))
            {
                foreach (var item in tree)
                {
                    if (item.Key == Convert.ToInt32(id))
                        a = item;
                }
            }
            using (FileStream fs = new FileStream("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/Author_Document", FileMode.Open))
            {
                fs.Seek(a.Value, 0);
                using (BinaryReader br = new BinaryReader(fs))
                {
                    list.Add(new Author_DocumentEntity()
                    {
                        A_D_id = br.ReadInt32().ToString(),
                        Author_id = br.ReadInt32().ToString(),
                        Document_id = br.ReadInt32().ToString()
                    });
                }
            }
            return list;
        }

        public void Edit(Author_DocumentEntity a_d)
        {
            if (a_d.Author_id == null)
                throw new ArgumentNullException();
            foreach (var item in a_d.Author_id)
            {
                if (!Char.IsNumber(item))
                    throw new FormatException();
            }
            if (a_d.Document_id == null)
                throw new ArgumentNullException();
            foreach (var item in a_d.Document_id)
            {
                if (!Char.IsNumber(item))
                    throw new FormatException();
            }
            Delete(a_d.A_D_id);

            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.Never;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeAuthor_Document");
            using (FileStream fs = new FileStream("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/Author_Document", FileMode.OpenOrCreate))
            {
                using (var tree = new BPlusTree<int, int>(options))
                {
                    tree.Add(Convert.ToInt32(a_d.A_D_id), (int)fs.Length);
                }
                fs.Seek((int)fs.Length, 0);

                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(Convert.ToInt32(a_d.A_D_id));
                    bw.Write(Convert.ToInt32(a_d.Author_id));
                    bw.Write(Convert.ToInt32(a_d.Document_id));
                }
            }
        }

        public Dictionary<string, string> Details(string a_d_id)
        {
            var list = new List<String>();
            var dic = new Dictionary<string, string>();

            var listAD = Find(a_d_id);
            DefaultPostgresProviderAuthor author = new DefaultPostgresProviderAuthor();
            DefaultPostgresProviderDocument document = new DefaultPostgresProviderDocument();
            var listA = author.Find(listAD.First().Author_id.ToString());
            var listD = document.Find(listAD.First().Document_id.ToString());
            dic.Add(listA.First().Name.ToString(), listD.First().Title.ToString());
            return dic;
        }

    }
}