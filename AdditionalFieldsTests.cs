using Microsoft.VisualStudio.TestTools.UnitTesting;
using NibernateAditionalFields.Domain;
using System;

namespace NibernateAditionalFields
{
    [TestClass]
    public class AdditionalFieldsTests : NHTestBase
    {
        [TestMethod]
        public void QuickThrowAwaySessionTest()
        {
            bool successful = false;
            try
            {
                using (var session = OpenSession())
                {
                    successful = true;
                }
            }
            catch 
            {
            }

            Assert.IsTrue(successful, "Failed to open a test session, please make sure configurations are working as expected.");
        }

        [TestMethod]
        public void LoadEntityWithAdditionalFields() 
        {
            using (var session = OpenSession())
            {
                var contacts = session.QueryOver<Contact>()
                    .List<Contact>();

                Assert.AreEqual(2, contacts.Count);
                Assert.IsNotNull(contacts[0].Additional);
                Assert.AreEqual(3, contacts[0].Additional.Count);
            }
        }
    }
}
