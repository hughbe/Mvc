// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Internal;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.Core.Test.ModelBinding.Internal
{
    public class ValidationStackTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(ValidationStack.CutOff + 1)]
        public void ValidationStack_PushFalseWhenAlreadyIn(int preloadCount)
        {
            // Arrange
            var validationStack = new TestValidationStack();
            var model = "This is a value";

            PreLoad(preloadCount, validationStack);

            // Act & Assert
            Assert.True(validationStack.Push(model));
            Assert.False(validationStack.Push(model));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(ValidationStack.CutOff + 1)]
        public void ValidationStack_PushAndPop(int preloadCount)
        {
            // Arrange
            var validationStack = new TestValidationStack();
            var model = "This is a value";

            PreLoad(preloadCount, validationStack);

            // Act
            validationStack.Push(model);
            validationStack.Pop(model);

            // Assert
            Assert.False(validationStack.Contains(model));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(ValidationStack.CutOff + 1)]
        public void ValidationStack_PopNull(int preloadCount)
        {
            // Arrange
            var validationStack = new TestValidationStack();
            var model = "This is a value";

            PreLoad(preloadCount, validationStack);

            // Act
            validationStack.Push(model);
            // Poping null when it's not there must not throw
            validationStack.Pop(null);
        }

        [Fact]
        public void ValidationStack_MoveToHashSet()
        {
            // Arrange
            var size = ValidationStack.CutOff + 1;

            var validationStack = new TestValidationStack();
            var models = new List<Model>();
            for (var i = 0; i < size; i++)
            {
                models.Add(new Model { Position = i });
            }

            // Act & Assert

            foreach (var model in models)
            {
                validationStack.Push(model);
            }

            Assert.Equal(size, validationStack.Count);

            models.Reverse();
            foreach (var model in models)
            {
                validationStack.Pop(model);
            }

            Assert.Equal(0, validationStack.Count);
            Assert.True(validationStack.UsingHashSet());
        }

        private void PreLoad(int preload, ValidationStack stack)
        {
            for (int i = 0; i < preload; i++)
            {
                stack.Push(i);
            }
        }

        private class Model
        {
            public int Position { get; set; }
        }

        private class TestValidationStack : ValidationStack
        {
            public bool Contains(object model)
            {
                if (_hashSet != null)
                {
                    return _hashSet.Contains(model);
                }
                else
                {
                    return _list.Contains(model);
                }
            }

            public bool UsingHashSet()
            {
                return _hashSet != null;
            }
        }
    }
}
