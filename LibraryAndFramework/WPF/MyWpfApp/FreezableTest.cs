using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyWpfApp
{
    class FreezableTest
    {
    }


    public class People : Freezable
    {
        //Freezable继承自DependencyObject,同时添加了Freezable方法,用于冻结对象.

        public string Name {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(People));


        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
}
