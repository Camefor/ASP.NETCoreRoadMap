using System.Linq;
using System.Linq.Expressions;

namespace LambdaAndLINQ
{

    //Models
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
    }


    /// <summary>
    /// LINQ
    /// </summary>
    class Program
    {

        /// <summary>
        /// 根据类 类型和属性名称创建泛型Func委托
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="propertyName"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        private static Func<T, TProperty> GetFuncT<T, TProperty>(Type entityType, string propertyName)
        {
            Func<T, TProperty> getPropertyDelegate = GetExpression<T, TProperty>(entityType, propertyName).Compile();
            return getPropertyDelegate;
        }

        /// <summary>
        /// 根据类 类型和属性名称创建 Expression TDelegate> 类
        /// 将强类型化的 Lambda 表达式表示为表达式树形式的数据结构。
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="propertyName"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        private static Expression<Func<T, TProperty>> GetExpression<T, TProperty>(Type entityType, string propertyName)
        {
            // 创建实体类型参数
            ParameterExpression entityParameter = Expression.Parameter(entityType, "Property");
            // 创建属性访问表达式
            MemberExpression propertyExpression = Expression.Property(entityParameter, "Name");
            // 创建委托类型参数
            ParameterExpression delegateParameter = Expression.Parameter(entityType);
            // 创建委托表达式
            Expression<Func<T, TProperty>> lambdaExpression
                = Expression.Lambda<Func<T, TProperty>>(propertyExpression, delegateParameter);
            return lambdaExpression;
        }



        static void Main(string[] args)
        {
            // init data
            var people = new List<Person>
            {
                new Person{FirstName = "Kevin",LastName="Wang",Age = 26,Gender = "Male"},
                new Person{FirstName = "Rhys",LastName="Lee",Age = 22,Gender = "Male"},
                new Person{FirstName = "Tim",LastName="Huan",Age = 29,Gender = "Female"},
                new Person{FirstName = "Tom",LastName="Pet",Age = 31,Gender = "Female"}
            };


            var d = people.Select<Person, string>(c => c.FirstName);
            //people.Select为 泛型方法，参数是一个func泛型委托 (一个Person参数，返回值为string)
            //泛型类型为 Person类 类型 和 String类型

            //测试目的： 我要生成对应的泛型委托 当作该方法的参数：
            Func<Person, string> myFunc = new Func<Person, string>((Person p) => { return p.FirstName; });
            //变体
            //people.Select<Person, string>(myFunc);
            var d2 = people.Select(myFunc);//类型推断

        }
    }
}