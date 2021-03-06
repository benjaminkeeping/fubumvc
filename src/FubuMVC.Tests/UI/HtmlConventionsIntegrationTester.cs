using System.Collections.Generic;
using System.Web.Routing;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.StructureMap;
using FubuMVC.UI;
using FubuMVC.UI.Configuration;
using FubuMVC.UI.Tags;
using HtmlTags;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.UI
{
    public class TestHtmlConventions : HtmlConventionRegistry
    {
        public TestHtmlConventions()
        {
            Editors.IfPropertyIs<string>().UseTextbox();
            Editors.IfPropertyIs<bool>().BuildBy(req => new CheckboxTag(req.Value<bool>()));
            Editors.AddClassForAttribute<FakeRequiredAttribute>("required");
            Editors.ModifyForAttribute<FakeMaximumStringLength>((tag, att) => tag.Attr("maxlength", att.MaxLength));

            Labels.Always.BuildBy(req => new HtmlTag("span").Text(req.Accessor.Name));
            Labels.Always.AddClass("label");

            Displays.Always.BuildBy(req => new HtmlTag("span").Text(req.StringValue()));
        }
    }


    [TestFixture]
    public class HtmlConventionsIntegrationTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry(x => x.HtmlConvention<TestHtmlConventions>());
            container = new Container(x => x.For<IFubuRequest>().Singleton());
            var facility = new StructureMapContainerFacility(container);

            new FubuBootstrapper(facility, registry).Bootstrap(new List<RouteBase>());

            var request = container.GetInstance<IFubuRequest>();

            address = new Address();
            request.Set(address);

            request.Get<Address>().ShouldBeTheSameAs(address);

            generator = container.GetInstance<TagGenerator<Address>>();
        }

        #endregion

        private Address address;
        private TagGenerator<Address> generator;
        private Container container;

        [Test]
        public void add_class_for_presence_of_an_attribute_negative_case()
        {
            generator.InputFor(x => x.Address2).ShouldNotHaveClass("required");
        }

        [Test]
        public void add_class_for_presence_of_an_attribute_positive_case()
        {
            generator.InputFor(x => x.Address1).ShouldHaveClass("required");
        }

        [Test]
        public void build_a_label()
        {
            HtmlTag tag = generator.LabelFor(x => x.City);
            tag.TagName().ShouldEqual("span");
            tag.Text().ShouldEqual("City");
            tag.ShouldHaveClass("label");
        }

        [Test]
        public void build_a_simple_display_for_a_non_null_string()
        {
            address.City = "Austin";
            HtmlTag tag = generator.DisplayFor(x => x.City);

            tag.ShouldHaveTagName("span");
            tag.Text().ShouldEqual("Austin");
        }

        [Test]
        public void build_a_simple_display_for_a_null_value()
        {
            address.City = null;
            HtmlTag tag = generator.DisplayFor(x => x.City);

            tag.ShouldHaveTagName("span");
            tag.Text().ShouldBeEmpty();
        }

        [Test]
        public void build_a_textbox_for_a_null_value()
        {
            generator.InputFor(x => x.City).Attr("value").ShouldBeEmpty();
        }

        [Test]
        public void build_a_textbox_for_a_string_value()
        {
            address.City = "Austin";

            HtmlTag tag = generator.InputFor(x => x.City);
            tag.ShouldHaveTagName("input");
            tag.Attr("type").ShouldEqual("text");

            tag.Attr("value").ShouldEqual("Austin");
        }

        [Test]
        public void by_default_the_html_conventions_should_use_the_default_element_naming_convention()
        {
            container.GetInstance<IElementNamingConvention>()
                .ShouldBeOfType<DefaultElementNamingConvention>();
        }

        [Test]
        public void modify_for_an_attribute_negative_case()
        {
            generator.InputFor(x => x.StateOrProvince).HasAttr("maxlength").ShouldBeFalse();
        }

        [Test]
        public void modify_for_an_attribute_positive_case()
        {
            generator.InputFor(x => x.Address1).Attr("maxlength").ShouldEqual("250");
        }

        [Test]
        public void name_is_placed_on_input_tags_by_default()
        {
            generator.InputFor(x => x.City).Attr("name").ShouldEqual("City");
            generator.InputFor(x => x.IsActive).Attr("name").ShouldEqual("IsActive");
        }
    }
}