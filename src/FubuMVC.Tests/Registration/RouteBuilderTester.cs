using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Routes;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class when_creating_the_route_for_an_input_from_a_good_pattern_with_no_defaults
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            thePattern = "here/there/{Name}/{Age}";
            route = RouteBuilder.Build<InputModel>(thePattern);
        }

        #endregion

        private RouteDefinition<InputModel> route;
        private string thePattern;

        [Test]
        public void has_a_route_input_for_each_property_in_the_pattern()
        {
            route.RouteInputs.Select(x => x.Name).ShouldHaveTheSameElementsAs("Name", "Age");
        }

        [Test]
        public void set_the_pattern_correctly()
        {
            route.Pattern.ShouldEqual(thePattern);
        }
    }

    [TestFixture]
    public class when_creating_a_route_for_an_input_from_a_pattern_with_invalid_property_name
    {
        [Test]
        public void should_throw_a_fubu_exception()
        {
            const string thePattern = "here/there/{Name}/{DoesNotExist}";

            Exception<FubuException>.ShouldBeThrownBy(() => RouteBuilder.Build<InputModel>(thePattern));
        }
    }


    [TestFixture]
    public class when_creating_the_route_for_an_input_from_a_good_pattern_with_no_defaults_from_the_type
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            thePattern = "here/there/{Name}/{Age}";
            route = RouteBuilder.Build(typeof (InputModel), thePattern).ShouldBeOfType<RouteDefinition<InputModel>>();
        }

        #endregion

        private RouteDefinition<InputModel> route;
        private string thePattern;

        [Test]
        public void has_a_route_input_for_each_property_in_the_pattern()
        {
            route.RouteInputs.Select(x => x.Name).ShouldHaveTheSameElementsAs("Name", "Age");
        }

        [Test]
        public void set_the_pattern_correctly()
        {
            route.Pattern.ShouldEqual(thePattern);
        }
    }

    [TestFixture]
    public class when_creating_the_route_for_an_input_from_a_good_pattern_with_defaults
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            thePattern = "here/there/{Name:Josh}/{Age}";
            route = RouteBuilder.Build<InputModel>(thePattern);
        }

        #endregion

        private string thePattern;
        private RouteDefinition<InputModel> route;

        [Test]
        public void pick_up_the_default_value_if_it_exists()
        {
            route.RouteInputs.First(x => x.Name == "Name").DefaultValue.ShouldEqual("Josh");
        }

        [Test]
        public void should_have_a_null_default_for_input_that_has_no_default_in_the_pattern()
        {
            route.RouteInputs.First(x => x.Name == "Age").DefaultValue.ShouldBeNull();
        }
    }

    public class InputModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public int Age2 { get; set; }
    }
}