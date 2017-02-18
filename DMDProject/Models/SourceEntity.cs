using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using System.IO;

namespace DMDProject.Models
{
    public class SourceEntity
    {
        public string Source_id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }

    }

    public class MyOrderingClass : IComparer<SourceEntity>
    {
        public int Compare(SourceEntity x, SourceEntity y)
        {
            int compareDate = x.Title.CompareTo(y.Title);
            if (compareDate == 0)
            {
                return x.Source_id.CompareTo(y.Source_id);
            }
            return compareDate;
        }
    }

    //repository
    public interface ISourceEntityProvider
    {
        IList<SourceEntity> GetAllEntities();
        void Insert(SourceEntity source);
        void Delete(string Source_id);
        IList<SourceEntity> Find(string id);
        void Edit(SourceEntity source);
    }


    public class DefaultPostgresProvider : ISourceEntityProvider
    {
        BPlusTree<int, int>.OptionsV2 options = new BPlusTree<int, int>.OptionsV2(PrimitiveSerializer.Int32, PrimitiveSerializer.Int32);

        private readonly string _connectionString;
        public DefaultPostgresProvider(string connectionString)
        {
            _connectionString = connectionString;
        }


        public DefaultPostgresProvider(NpgsqlConnectionStringBuilder connectionStringBuilder)
            : this(connectionStringBuilder.ConnectionString)
        {
        }
        public IList<SourceEntity> GetAllEntities()
        {
            var list = new List<SourceEntity>();
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.Never;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeSource");
            int count = 0;
            using (var tree = new BPlusTree<int, int>(options))
            {
                using (FileStream fs = new FileStream("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/Source", FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        foreach (var item in tree)
                        {
                            //if (count == 50)
                            //{
                            //    IComparer<SourceEntity> comparer = new MyOrderingClass();
                            //    list.Sort(comparer);
                            //    return list;
                            //}
                            fs.Seek(item.Value, 0);
                            list.Add(new SourceEntity()
                            {
                                Source_id = br.ReadInt32().ToString(),
                                Title = br.ReadString(),
                                Type = br.ReadString()
                            });
                            count++;
                        }
                    }
                }
            }
            IComparer<SourceEntity> comparer = new MyOrderingClass();
            list.Sort(comparer);
            return list;
        }

        public void Insert(SourceEntity source)
        {
            if (source.Title == null || source.Type == null)
                throw new ArgumentNullException();
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.Never;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeSource");
            using (FileStream fs = new FileStream("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/Source", FileMode.OpenOrCreate))
            {
                using (var tree = new BPlusTree<int, int>(options))
                {
                    KeyValuePair<int, int> kv = tree.Last();
                    int key = kv.Key;
                    key++;
                    source.Source_id = key.ToString();
                    tree.Add(Convert.ToInt32(source.Source_id), (int)fs.Length);
                }
                fs.Seek((int)fs.Length, 0);

                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(Convert.ToInt32(source.Source_id));
                    bw.Write(source.Title);
                    bw.Write(source.Type);
                }
            }
        }

        public void Delete(string Source_id)
        {
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.Never;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeSource");
            using (var tree = new BPlusTree<int, int>(options))
            {
                tree.Remove(Convert.ToInt32(Source_id));
            }
        }

        public IList<SourceEntity> Find(string id)
        {
            var list = new List<SourceEntity>();
            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.IfNeeded;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeSource");
            KeyValuePair<int, int> a = new KeyValuePair<int, int>();
            using (var tree = new BPlusTree<int, int>(options))
            {
                foreach (var item in tree)
                {
                    if (item.Key == Convert.ToInt32(id))
                        a = item;
                }
            }
            using (FileStream fs = new FileStream("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/Source", FileMode.Open))
            {
                fs.Seek(a.Value, 0);
                using (BinaryReader br = new BinaryReader(fs))
                {
                    list.Add(new SourceEntity()
                    {
                        Source_id = br.ReadInt32().ToString(),
                        Title = br.ReadString(),
                        Type = br.ReadString()
                    });
                }
            }
            return list;
        }

        public void Edit(SourceEntity source)
        {
            if (source.Title == null || source.Type == null)
                throw new ArgumentNullException();
            Delete(source.Source_id);

            options.CalcBTreeOrder(4, 4);
            options.CreateFile = CreatePolicy.Never;
            options.FileName = Path.GetFileName("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/TreeSource");
            using (FileStream fs = new FileStream("C:/Users/Дмитрий/Desktop/DMDProject/DMDProject/Source", FileMode.OpenOrCreate))
            {
                using (var tree = new BPlusTree<int, int>(options))
                {
                    tree.Add(Convert.ToInt32(source.Source_id), (int)fs.Length);
                }
                fs.Seek((int)fs.Length, 0);

                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(Convert.ToInt32(source.Source_id));
                    bw.Write(source.Title);
                    bw.Write(source.Type);
                }
            }
        }
    }
}