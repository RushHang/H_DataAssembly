﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataLibraries.GetAndSet;
using TestModel;
using System.Reflection;
using DataLibraries;
using System.Threading;
using System.Data;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //int i = 5, k; k = ++i + i--;

            //Factory.peiz();
            //Test1 s2 = new Test1();
            //Type type = s2.GetType();
            //foreach (var s in type.GetProperties())
            //{
            //    IGetValue getv = s.CreatePropertyGetterWrapper();
            //    ISetValue setv = s.CreatePropertySetterWrapper();
            //    setv.Set(s2, ms());
            //    Console.WriteLine(getv.Get(s2));
            //}
            //TryParseDelegate tryp = new TryParseDelegate(TryPasrse.TryInt32);
            //Console.WriteLine(tryp("sad"));
            //PropertyInfo pro = type.GetProperty("Code");
            //IGetValue gets = pro.CreatePropertyGetterWrapper();
            //ISetValue sets = pro.CreatePropertySetterWrapper();

            //sets.Set(s2, "asf");
            //Console.Write(gets.Get(s2));

            //(new Program()).ms(s2);
            //Console.Write(s2.Name);
            IDBOperate oper = Factory.Da.CreateOperate();
            //DataTable dt = oper.QueryDt("select * from user");
            //IList<TestUser> list = oper.QueryList<TestUser>("select * from user");
            //TestUser user = new TestUser();
            //user.UserName = "tfg";
            //oper.Insere(user);
            REPORT_DEPTModel tmodel = oper.Get<REPORT_DEPTModel>("b8a51aee154742efb22ba0c94060eaae");
            tmodel.Type_ID = "5";
            tmodel.REPORT_NAME = "棉毛裤";
            oper.Update(tmodel);
            int count = 0;
            IList<REPORT_DEPTModel> list = oper.QueryList<REPORT_DEPTModel>(1, 2, out count,null);
            //IList<TreeModel> lists = oper.QueryList<TreeModel>("select Id,Content from Tree");
            //IList<string> sl = dt.ToList<String>();
            //DataTable dt = lists.ToDataTable<TreeModel>();
            foreach(var item in list)
            {
                Console.WriteLine(item.ID);
            }
            Console.Read();
        }

        static object ms()
        {
            return 123;
        }
    }

    public class Test1
    {
        public int Id { get; set; }


    }
}
