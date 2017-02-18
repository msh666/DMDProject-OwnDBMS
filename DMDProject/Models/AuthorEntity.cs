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
    public class AuthorEntity
    {
        public string Author_id { get; set; }
        public string Name { get; set; }
        public string Research_area { get; set; }
        public string Institution { get; set; }

    }

    public class AuthorOrdering : IComparer<AuthorEntity>
    {
        public int Compare(AuthorEntity x, AuthorEntity y)
        {
            int compareDate = y.Research_area.CompareTo(x.Research_area);
            if (compareDate == 0)
            {
                return x.Author_id.CompareTo(y.Author_id);
            }
            return compareDate;
        }
    }
    public interface IAuthorEntityProvider
    {
        IList<AuthorEntity> GetAllEntities();
        void Insert(AuthorEntity author);
        void Delete(string Author_id);
        IList<AuthorEntity> Find(string Author_id);
        void Edit(AuthorEntity author);
        IList<DocumentEntity> Details(string id);
        IList<AuthorEntity> Rate();
        IList<AuthorEntity> Search(string Name, string Research_area, string Institution);
    }

    public class DefaultPostgresProviderAuthor: IAuthorEntityProvider
    {
        BPlusTree<int, int>.OptionsV2 options = new BPlusTree<int, int>.OptionsV2(PrimitiveSerializer.Int32, PrimitiveSerializer.Int32);

        public IList<AuthorEntity> GetAllEntities()
        {
            var list = new List<AuthorEntity>();
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.Never;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeAuthor");
            int count = 0;
            using (var tree = new BPlusTree<int, int>(options))
            {
                using (FileStream fs = new FileStream("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/Author", FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        foreach (var item in tree)
                        {
                            if (count == 50)
                                return list;
                            fs.Seek(item.Value, 0);
                            list.Add(new AuthorEntity()
                            {
                                Author_id = br.ReadInt32().ToString(),
                                Name = br.ReadString(),
                                Research_area = br.ReadString(),
                                Institution = br.ReadString(),
                            });
                            count++;
                        }
                    }
                }
            }
            return list;
        }

        public void Insert(AuthorEntity author)
        {
            if (author.Name == null)
                throw new ArgumentNullException();
            if (author.Research_area == null)
                author.Research_area = " ";
            if (author.Institution == null)
                author.Institution = " ";
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.Never;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeAuthor");
            using (FileStream fs = new FileStream("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/Author", FileMode.OpenOrCreate))
            {
                using (var tree = new BPlusTree<int, int>(options))
                {
                    KeyValuePair<int, int> kv = tree.Last();
                    int key = kv.Key;
                    key++;
                    author.Author_id = key.ToString();
                    tree.Add(Convert.ToInt32(author.Author_id), (int)fs.Length);
                }
                fs.Seek((int)fs.Length, 0);

                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(Convert.ToInt32(author.Author_id));
                    bw.Write(author.Name);
                    bw.Write(author.Research_area);
                    bw.Write(author.Institution);
                }
            }
        }

        public void Delete(string Author_id)
        {
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.Never;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeAuthor");
            using (var tree = new BPlusTree<int, int>(options))
            {
                tree.Remove(Convert.ToInt32(Author_id));
            }
        }

        public IList<AuthorEntity> Find(string id)
        {
            var list = new List<AuthorEntity>();
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.IfNeeded;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeAuthor");
            KeyValuePair<int, int> a = new KeyValuePair<int, int>();
            using (var tree = new BPlusTree<int, int>(options))
            {
                foreach (var item in tree)
                {
                    if (item.Key == Convert.ToInt32(id))
                        a = item;
                }
            }
            using (FileStream fs = new FileStream("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/Author", FileMode.Open))
            {
                fs.Seek(a.Value, 0);
                using (BinaryReader br = new BinaryReader(fs))
                {
                    list.Add(new AuthorEntity()
                    {
                        Author_id = br.ReadInt32().ToString(),
                        Name = br.ReadString(),
                        Research_area = br.ReadString(),
                        Institution = br.ReadString(),
                    });
                }
            }
            return list;
        }

        public void Edit(AuthorEntity author)
        {
            if (author.Name == null)
                throw new ArgumentNullException();
            if (author.Research_area == null)
                author.Research_area = " ";
            if (author.Institution == null)
                author.Institution = " ";
            Delete(author.Author_id);

            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.Never;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeAuthor");
            using (FileStream fs = new FileStream("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/Author", FileMode.OpenOrCreate))
            {
                using (var tree = new BPlusTree<int, int>(options))
                {
                    tree.Add(Convert.ToInt32(author.Author_id), (int)fs.Length);
                }
                fs.Seek((int)fs.Length, 0);

                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(Convert.ToInt32(author.Author_id));
                    bw.Write(author.Name);
                    bw.Write(author.Research_area);
                    bw.Write(author.Institution);
                }
            }
        }


        public IList<Author_DocumentEntity> FindAuthors(string id)
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
                            if (Author_id == id)
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

        public IList<DocumentEntity> Details(string id)
        {
            DefaultPostgresProviderDocument doc = new DefaultPostgresProviderDocument();
            var author_document = FindAuthors(id);
            List<DocumentEntity> document = new List<DocumentEntity>();
            foreach (var item in author_document)
            {
                var d = doc.Find(item.Document_id.ToString());
                document.Add(d.First());
            }
            IList<DocumentEntity> docs = document;                
            return docs;
        }

        public IList<AuthorEntity> FindAuthorsRate(string id)
        {
            var list = new List<AuthorEntity>();
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.IfNeeded;
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
                            fs.Seek(item.Value, 0);

                            string A_D_id = br.ReadInt32().ToString();
                            string Author_id = br.ReadInt32().ToString();
                            string Document_id = br.ReadInt32().ToString();
                            if (Author_id == id)
                            {
                                count++;
                            }
                        }
                        list.Add(new AuthorEntity()
                        {
                            Author_id = id,
                            Name = "",
                            Research_area = count.ToString(),
                            Institution = ""
                        });
                    }
                }
            }
            return list;
        }

        public IList<AuthorEntity> Rate()
        {
            var list = new List<AuthorEntity>();
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.Never;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeAuthor");
            int count = 0;
            using (var tree = new BPlusTree<int, int>(options))
            {
                
                foreach (var item in tree)
                {
                    if(count == 10)
                    {
                        IComparer<AuthorEntity> comparer = new AuthorOrdering();
                        list.Sort(comparer);
                        return list;
                    }
                    count++;
                    list.Add(FindAuthorsRate(item.Key.ToString()).First());
                }
            }
            return list;
        }

        public IList<AuthorEntity> Search(string name, string research_area, string institution)
        {
            if (name == null)
                name = "";
            if (research_area == null)
                research_area = "";
            if (institution == null)
                institution = "";
            var list = new List<AuthorEntity>();
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.Never;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeAuthor");
            using (var tree = new BPlusTree<int, int>(options))
            {
                using (FileStream fs = new FileStream("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/Author", FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        foreach (var item in tree)
                        {
                            fs.Seek(item.Value, 0);
                            string Author_id = br.ReadInt32().ToString();
                            string Name = br.ReadString();
                            string Research_area = br.ReadString();
                            string Institution = br.ReadString();
                            if (Name.Contains(name))
                                if (Research_area.Contains(research_area))
                                    if (Institution.Contains(institution))
                                        list.Add(new AuthorEntity()
                                        {
                                            Author_id = Author_id,
                                            Name = Name,
                                            Research_area = Research_area,
                                            Institution = Institution
                                        });
                        }
                    }
                }
            }
            return list;
        }
    }
}