using EncodingService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EncodingTests
{
    [TestClass]
    public class Base64Test
    {
        [TestMethod]
        public void Test_80b898c6aec057YaccAL4QgXKORW_Encode_to_Decode()
        {
            var target = new Base64();

            var plaintext = "80b898c6aec057YaccAL4QgXKORW";
            var encode = target.Encode(plaintext);
            var actual = target.Decode(encode);

            Assert.AreEqual(plaintext, actual);
        }
    }
}