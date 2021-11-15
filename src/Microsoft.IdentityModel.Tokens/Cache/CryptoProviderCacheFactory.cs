﻿//------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.CryptoProviderCacheOptions
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System;
using Microsoft.IdentityModel.Logging;

namespace Microsoft.IdentityModel.Tokens
{
    /// <summary>
    /// The provider cache factory that creates crypto provider caches.
    /// </summary>
    internal class CryptoProviderCacheFactory
    {
        internal static CryptoProviderCache Create() => Create(new CryptoProviderCacheOptions());

        /// <summary>
        /// Create a new instance of InMemoryCryptoProviderCache and the _signingSignatureProviders and _verifyingSignatureProviders caches based on the cache type in _cryptoProviderCacheOptions.
        /// </summary>
        /// <param name="cryptoProviderCacheOptions">Specifies the options which can be used to configure the internal cryptoprovider cache.</param>
        internal static CryptoProviderCache Create(CryptoProviderCacheOptions cryptoProviderCacheOptions)
        {
            if (cryptoProviderCacheOptions == null)
                throw LogHelper.LogArgumentNullException(nameof(cryptoProviderCacheOptions));

            IProviderCache<string, SignatureProvider> signingProvidersCache;
            IProviderCache<string, SignatureProvider> verifyinfProvidersCache;

            // Create the signature provider caches based on the ProviderCacheType.
            if (cryptoProviderCacheOptions.CacheType == ProviderCacheType.LRU)
            {
                signingProvidersCache = new EventBasedLRUCache<string, SignatureProvider>(cryptoProviderCacheOptions, StringComparer.Ordinal) { OnItemRemoved = (SignatureProvider signatureProvider) => signatureProvider.CryptoProviderCache = null };
                verifyinfProvidersCache = new EventBasedLRUCache<string, SignatureProvider>(cryptoProviderCacheOptions, StringComparer.Ordinal) { OnItemRemoved = (SignatureProvider signatureProvider) => signatureProvider.CryptoProviderCache = null };
            }
            else
            {
                signingProvidersCache = new RandomEvictCache<string, SignatureProvider>(cryptoProviderCacheOptions, StringComparer.Ordinal) { OnItemRemoved = (SignatureProvider signatureProvider) => signatureProvider.CryptoProviderCache = null };
                verifyinfProvidersCache = new RandomEvictCache<string, SignatureProvider>(cryptoProviderCacheOptions, StringComparer.Ordinal) { OnItemRemoved = (SignatureProvider signatureProvider) => signatureProvider.CryptoProviderCache = null };
            }

            return new InMemoryCryptoProviderCache(cryptoProviderCacheOptions, signingProvidersCache, verifyinfProvidersCache);
        }
    }
}
