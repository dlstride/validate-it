using RabbitMQValidator.Validators;
using System;
using Xunit;

namespace RabbitMQValidatorTests
{
    public class RabbitMQValidatorProxyTests
    {
        [Theory, AutoData_NSubstitute]
        public void RabbitMQValidatorProxyInitWithNullParametersThrow()
        {
            Assert.ThrowsAny<Exception>(() => { RabbitMQValidatorProxy rmq = new RabbitMQValidatorProxy(null, null); });
        }
    }
}