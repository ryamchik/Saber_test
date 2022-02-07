using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    class ListNode
    {
        public ListNode Previous;
        public ListNode Next;
        public ListNode Random; // произвольный элемент внутри списка
        public string Data;
    }


    class ListRandom
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;
        static Random rand = new Random();

        public ListNode AddNode(ListNode previous)
        {
            ListNode newnode = new ListNode();
            newnode.Previous = previous;
            newnode.Data = DataFactory();
            previous.Next = newnode;

            return newnode;
        }

        public string DataFactory()
        {
            string data =  rand.Next(0, 1000).ToString();

            return data;
        }

        private ListNode GiveMeNode(int index)
        {
            int counter = 0;

            for (ListNode current = Head; current.Next != null; current = current.Next)
            {
                if (counter == index)
                    return current;

                counter++;
            }

            return new ListNode();
        }

        public void Randominator()
        {
            var arr = new List<ListNode>();

            for (ListNode current = Head; current != null; current = current.Next)
            {
                arr.Add(current);
            }
            
            for (ListNode current = Head; current != null; current = current.Next)
            {
                int n = rand.Next(1, Count);
                current.Random = arr[n];
            }

        }

        public void Serialize(Stream s)
        {
            Dictionary<ListNode, int> dictionary = new Dictionary<ListNode, int>();
            int id = 0;

            for (ListNode current = Head; current != null; current = current.Next)
            {
                dictionary.Add(current, id);
                id++;
            }

            using (StreamWriter writer = new StreamWriter(s))
            {
                for (ListNode current = Head; current != null; current = current.Next)
                {
                    writer.WriteLine(current.Data + ":"+ Convert.ToString(dictionary[current.Random]));
                }
            }

            Console.WriteLine("\n   Сериализация произведена успешно!\n");
        }

        public void Deserialize(Stream s)
        {
            int counter = 0;
            string line;
            var arr = new List<string>();

            
            try
            {
                using (StreamReader reader = new StreamReader(s))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!line.Equals(""))
                        {
                            arr.Add(line);
                            counter++;
                        }
                    }

                    Console.WriteLine("\n   Чтение завершено!\n");
                }

                Count = counter;
                Head = new ListNode();
                ListNode current = Head;

                for (int i = 0; i < Count; i++)
                {
                    current.Data = arr[i];
                    current.Next = new ListNode();

                    if (i == (this.Count - 1))
                        Tail = current;
                    else
                    {
                        current.Next.Previous = current;
                        current = current.Next;
                    }
                }

                for (ListNode currentN = Head; currentN.Next != null; currentN = currentN.Next)
                {
                    currentN.Random = GiveMeNode(Convert.ToInt32(currentN.Data.Split(':')[1]));
                    currentN.Data = currentN.Data.Split(':')[0];
                }

                Console.WriteLine("\n   Десериализация произведена успешно!\n");
            }
            catch (Exception e)
            {
                Console.WriteLine("\n   Ошибка при десериализации!\n");
            }
        }
    }


    class Program
    {
        static bool Test(ListRandom original, ListRandom fromFile)
        {
            bool test = false;

            if (original.Count == fromFile.Count)
            {
                var arr1 = new List<ListNode>();
                var arr2 = new List<ListNode>();
                bool ok = false;

                for (ListNode cur1 = original.Head, cur2 = fromFile.Head; cur1.Next != null && cur2.Next != null; cur1 = cur1.Next, cur2 = cur2.Next)
                {
                    arr1.Add(cur1);
                    arr2.Add(cur2);
                }

                for (int i = 0; i < arr1.Count; i++)
                {
                    if (arr1[i].Data == arr2[i].Data && arr1[i].Random.Data == arr2[i].Random.Data)
                        if (arr1[i].Next != null && arr2[i].Next != null)
                            if (arr1[i].Next.Data == arr2[i].Next.Data)
                                if (arr1[i].Previous != null && arr2[i].Previous != null)
                                    if (arr1[i].Previous.Data == arr2[i].Previous.Data)
                                        ok = true;
                                    else
                                    {
                                        ok = false;
                                        break;
                                    }
                }

                test = (ok) ? true : false;
            }

            return test;
        }

        static void Main(string[] args)
        {
            ListRandom list = new ListRandom();
            list.Head = new ListNode();
            list.Head.Data = list.DataFactory();
            list.Tail = new ListNode();
            Random rand = new Random();
            list.Count = rand.Next(1, 1000);
            list.Tail = list.Head;

            for (int i = 1; i<list.Count; i++)
            {
                list.Tail = list.AddNode(list.Tail);
            }

            /* Было принято решение добавления ссылок на случайные
               уже после создания списка, так как задача не подразумевает
               добавления новых элементов по ходу выполнения алгоритма. */

            list.Randominator();

            /* Для проверки работы сериализации и десериализации было принято
               решение копирования сохданного списка и создания функции для тестирования.*/

            ListRandom listFile = new ListRandom();
            
            const string ExFile = "buf.txt";


            // Сериализация списка.
            using (FileStream fs = new FileStream(ExFile, FileMode.Create, FileAccess.Write))
                list.Serialize(fs);

            try
            {
                // Десериализация списка.
                using (FileStream fs = new FileStream(ExFile, FileMode.Open, FileAccess.Read))
                    listFile.Deserialize(fs);
            }
            catch(Exception e)
            {
                Console.WriteLine(" !!! {0}", e.Message);
            }

            bool test = Test(list, listFile);

            if (test)
                Console.WriteLine("\n Все прошло успешно!\n");
            else
                Console.WriteLine("\n Не все прошло успешно!\n");

            Console.ReadLine();
        }
    }
}
