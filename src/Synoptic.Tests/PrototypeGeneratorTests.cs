using System.Linq;
using NUnit.Framework;

namespace Synoptic.Tests
{
    [TestFixture]
    public class PrototypeGeneratorTests
    {
        [Test]
        public void should_generate_hyphened_name_when_user_not_supplied_prototype()
        {
            string proto = Use("Command1").ToOptionPrototype();
            Assert.That(proto, Is.EqualTo("param-one="));
        }

        [Test]
        public void should_append_default_suffix_when_user_supplied_prototype_does_not_have_it_and_mandatory()
        {
            string proto = Use("Command2").ToOptionPrototype();
            Assert.That(proto, Is.EqualTo("param="));
        }

        [Test]
        public void should_not_append_default_suffix_when_user_supplied_prototype_have_it_and_mandatory()
        {
            string proto = Use("Command3").ToOptionPrototype();
            Assert.That(proto, Is.EqualTo("param:"));
        }

        [Test]
        public void should_remove_and_suffix_when_user_supplied_prototype_have_it_and_not_mandatory()
        {
            string proto = Use("Command4").ToOptionPrototype();
            Assert.That(proto, Is.EqualTo("param"));
        }

        private static ParameterInfoWrapper Use(string name)
        {
            return new ParameterInfoWrapper(typeof(GeneratorTest).GetMethod(name).GetParameters().First());
        }

        public class GeneratorTest
        {
            public void Command1(string paramOne) { }

            public void Command2([CommandParameter("param")]string paramOne) { }

            public void Command3([CommandParameter("param:")]string paramOne) { }

            public void Command4([CommandParameter("param=")]bool paramOne) { }
        }
    }
}