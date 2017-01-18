# 1. Requirements

- Language agnostic. It can't be a programming library.
- SDKs should be able to redirect traffic for test purpose.

# 2. Test Proxy Server

```
[SDK] <-> [Test Proxy] <-> [Azure Service]
           | | | 
           | | Swagger
           | Recording (HTTP logging)
           Logging
```

See also Ruby VCR Recording.

# 3. Coverage N x M matrix

O(N + M) vs O(N * M)

# 4. Current Testing VS Proposal

Current

```
[Swagger] -> [UnitTest] -> [Test 1] -> [Test 2] 
          -> [UnitTest] -------------> [Test 3]  
```

Proposed

```
[Swagger] -> [UnitTest] -> [Test 1] -> [Test 2] -> [Proposed SDK Test, almost E2E] 
          -> [UnitTest] -------------> [Test 3]  
```

# 5. Requirements for Implementing Azure SDK for a new language

1. Swagger Code Generator
2. Test Case Code Generator
3. Execution Script
