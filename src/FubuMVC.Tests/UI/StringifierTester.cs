using System;
using FubuMVC.Core;
using FubuMVC.UI;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    [TestFixture]
    public class StringifierTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            stringifier = new Stringifier();
        }

        #endregion

        private Stringifier stringifier;

        public interface Something
        {
            string Description { get; }
        }

        public class RedSomething : Something
        {
            public string Description { get { return "Red"; } }
        }

        public class BlueSomething : Something
        {
            public string Description { get { return "Blue"; } }
        }

        [Test]
        public void get_date_value_out_of_the_box()
        {
            DateTime date = DateTime.Today;
            stringifier.GetString(typeof (DateTime), date).ShouldEqual(date.ToString());
        }

        [Test]
        public void get_int_values_out_of_the_box()
        {
            stringifier.GetString(typeof (int), 123).ShouldEqual(123);
        }

        [Test]
        public void get_string_values_out_of_the_box()
        {
            stringifier.GetString(typeof (string), null).ShouldBeEmpty();
            stringifier.GetString(typeof (string), "a").ShouldEqual("a");
        }

        [Test]
        public void register_a_string_conversion_for_a_class()
        {
            stringifier.ForClass<Address>(a => "{0}, {1}".ToFormat(a.Address1, a.City));

            var address = new Address
            {
                Address1 = "2050 Ozark",
                City = "Joplin"
            };

            stringifier.GetString(typeof (Address), address).ShouldEqual("2050 Ozark, Joplin");
        }

        [Test]
        public void register_a_string_conversion_for_a_series_of_types()
        {
            stringifier.ForTypesOf<Something>(s => s.Description);

            stringifier.GetString(typeof (RedSomething), new RedSomething()).ShouldEqual("Red");
            stringifier.GetString(typeof (RedSomething), new BlueSomething()).ShouldEqual("Blue");
        }

        [Test]
        public void register_a_string_conversion_function_by_a_struct_type()
        {
            stringifier.ForStruct<DateTime>(d => d.ToShortDateString());

            DateTime date = DateTime.Today;
            stringifier.GetString(typeof (DateTime), date).ShouldEqual(date.ToShortDateString());
        }
    }
}