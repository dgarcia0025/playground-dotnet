﻿//===============================================================================
// Microsoft patterns & practices
// Unity Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.PolicyInjection
{
    /// <summary>
    /// Summary description for PolicyInjectionWithGenericMethodsFixture
    /// </summary>
    [TestClass]
    public class PolicyInjectionWithGenericMethodsFixture
    {
        [TestCleanup]
        public void Teardown()
        {
            GlobalCountCallHandler.Calls.Clear();
        }

        [TestMethod]
        public void TransparentProxyCanInterceptNonGenericMethod()
        {
            CanInterceptNonGenericMethod<TransparentProxyInterceptor>();
        }

        [TestMethod]
        public void TransparentProxyCanInterceptGenericMethod()
        {
            CanInterceptGenericMethod<TransparentProxyInterceptor>();
        }

        [TestMethod]
        public void InterfaceInterceptorCanInterceptNonGenericMethod()
        {
            CanInterceptNonGenericMethod<InterfaceInterceptor>();
        }

        [TestMethod]
        public void InterfaceInterceptorCanInterceptGenericMethod()
        {
            CanInterceptGenericMethod<InterfaceInterceptor>();
        }

        [TestMethod]
        public void VirtualMethodCanInterceptNonGenericMethod()
        {
            CanInterceptNonGenericMethod<VirtualMethodInterceptor>();
        }

        [TestMethod]
        public void VirtualMethodCanInterceptGenericMethod()
        {
            CanInterceptGenericMethod<VirtualMethodInterceptor>();
        }

        private static IUnityContainer CreateContainer<TInterceptor>()
            where TInterceptor : IInterceptor
        {
            return new UnityContainer()
                .AddNewExtension<Interception>()
                .RegisterType<IInterfaceWithGenericMethod, MyClass>(
                    new Interceptor<TInterceptor>(),
                    new InterceptionBehavior<PolicyInjectionBehavior>());
        }

        private static void CanInterceptNonGenericMethod<TInterceptor>()
            where TInterceptor : IInterceptor
        {
            var container = CreateContainer<TInterceptor>();

            var instance = container.Resolve<IInterfaceWithGenericMethod>();

            instance.DoSomethingElse("boo");

            Assert.AreEqual(1, GlobalCountCallHandler.Calls["NonGeneric"]);
        }

        private static void CanInterceptGenericMethod<TInterceptor>()
            where TInterceptor : IInterceptor
        {
            var container = CreateContainer<TInterceptor>();

            var instance = container.Resolve<IInterfaceWithGenericMethod>();

            instance.DoSomething<string>();
            instance.DoSomething<int>();

            Assert.AreEqual(2, GlobalCountCallHandler.Calls["Generic"]);
        }
    }

    public interface IInterfaceWithGenericMethod
    {
        [GlobalCountCallHandler(HandlerName = "Generic")]
        T DoSomething<T>();

        [GlobalCountCallHandler(HandlerName = "NonGeneric")]
        void DoSomethingElse(string param);
    }

    public class MyClass : IInterfaceWithGenericMethod
    {
        public virtual T DoSomething<T>()
        {
            return default(T);   
        }

        public virtual void DoSomethingElse(string param)
        {
        }
    }
}
