# WALTER
Introducing the WALTER Framework: **W**orkable **A**lgorithms for **L**ocation-aware **T**ransmission, **E**ncryption **R**esponse. 
Designed for modern developers, WALTER is a groundbreaking suite of NuGet packages crafted for excellence in .NET Standard 2.0, 2.1, Core 3.1, and .NET 6, 7, 8, as well as C++ environments. Emphasizing 100% AoT support and reflection-free operations, this framework is the epitome of performance and stability.

Whether you're tackling networking, encryption, or secure communication, WALTER offers unparalleled efficiency and precision in processing, making it an essential tool for developers who prioritize speed and memory management in their applications.

## About this Nuget Package
This NuGet package contains helpful extension methods for any .NET project targeting anything from the Web to MAUI, Native Windows, Linux, or Mac. Even though this NuGet package is not intended for direct use, it does come with some handy features.
You can download the cmpiled help file as well as see code samples in [github](https://github.com/vesnx/walter)

## Ahead-Of-Time (AOT) Compilation Compliant
The NuGet package can be used with projects that are trimming and use AoT. However, Newtonsoft Json is not AoT compliant as it uses reflection; therefore, Newtonsoft extension methods are excluded from AoT support.


### Json Extensions
We offer helpfull extension methods for when you need to process Large JSON files and can't load the whole file in memory as well as exception free processing of json files, strings and streams using the `IsValidJson<T>()` extension method

The `JsonStreamReader<T>` class in the `Walter` namespace is a powerful utility for deserializing JSON data from streams in .NET. 
This class provides a seamless way to read JSON content instance by instance, handling exceptions gracefully, 
and is compliant with various .NET versions including .NET Standard 2.0, 2.1, Core 3.1, and .NET 6, 7, 8. It's 
designed to work efficiently.

## Usage

The class is used for reading JSON data from a stream, with error handling mechanisms to capture any exceptions during the deserialization process.
### Process a file row by row so you do not have to have the whole file in memory while processing it.
```c#
using var stream = File.OpenRead("AppData\\LinkedIn.json");
var sr = new Walter.JsonStreamReader<TCPIPNetwork>(stream,TCPIPNetworkJsonContext.Default.TCPIPNetwork);
int i=0;
await foreach (TCPIPNetwork item in sr.ReadAsync())
{
    i++;
    if(sr.Errors.Count>0)
    {
        _logger.LogError(sr.Errors[i]
                        ,"File line {line}, failed so entry {i} is skipped as it failed due to {error}"
                        ,sr.Errors[i].LineNumber,i+1,sr.Errors[i].Message);
    }

}
```

### Process data from a webservice
The sample shows fetching a JSON stream from a web service streaming endpoint and processing the data row by row:
```c#
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;

// ... other necessary namespaces

public async Task ProcessJsonDataFromWebServiceAsync()
{
    using var httpClient = new HttpClient();
    try
    {
        var response = await httpClient.GetStreamAsync("https://example.com/api/datastream");
        
        var sr = new Walter.JsonStreamReader<TCPIPNetwork>(response, TCPIPNetworkJsonContext.Default.TCPIPNetwork);
        int i = 0;
        await foreach (TCPIPNetwork item in sr.ReadAsync())
        {
            i++;
            // Process each item
            // ...

            if (sr.Errors.Count > 0)
            {
                _logger.LogError(sr.Errors[i - 1],
                                 "File line {line}, failed so entry {i} is skipped as it failed due to {error}",
                                 sr.Errors[i - 1].LineNumber, i, sr.Errors[i - 1].Message);
            }
        }
    }
    catch (HttpRequestException e)
    {
        _logger.LogError("Error fetching data from web service: {Message}", e.Message);
    }
}
```

### Read a file in one time and process all entries skip those that would fail

```c#
using var stream = File.OpenRead("TestData\\LinkedIn.json");
var sr = new Walter.JsonStreamReader<List<TCPIPNetwork>>(stream,TCPIPNetworkListJsonContext.Default.ListTCPIPNetwork);
var list = sr.Read();

foreach (var item in list)
{
 ...
}
```
### String extension methods
This NuGet package includes a range of useful extension methods, enhancing functionality for JSON processing, conversions between hex and bytes, and handling SQL Server varbinary types. For additional examples and comprehensive documentation, you can visit the [GitHub repository](https://github.com/vesnx/walter).

#### `ToSqlBinaryString()` integration of Encrypted Data into T-SQL Command

The `encryptedBytes.ToSqlBinaryString()` method is utilized to convert the byte array 
(which is the result of the encryption process) into a SQL Server-friendly varbinary 
format. This format is compatible with T-SQL syntax and is necessary for correctly 
storing the encrypted data in the database.

The resulting string, prefixed with `0x`, represents the hexadecimal representation 
of the encrypted data. This string is directly embedded into the T-SQL command, 
allowing the encrypted data to be used in database operations such as INSERT or 
UPDATE statements. In this way, the encrypted data can be stored securely in the 
database while maintaining the ability to perform standard SQL operations on it.

```c#
var encryptedDataString = encryptedBytes.ToSqlBinaryString();
var tsql = @$"
    declare @EncryptedData Varbinary(64) = {encryptedDataString};
    ...
    INSERT INTO MyTable (EncryptedColumn) VALUES (@EncryptedData);
";
```
### Secure encryption and decryption

#### Sample Test Cases for Understanding the Deterministic Encryption

Deterministic encryption is a method where identical plain text values are always encrypted into identical cipher text. This approach is particularly useful in scenarios where you need to:

- **Store Sensitive Data on Third-party Servers**: Especially relevant under GDPR compliance when using cloud services where you don't have control over the underlying hardware or database encryption mechanisms.
- **Enable Grouping and Searching**: Deterministic encryption allows for the encrypted data to be searchable and groupable, which is essential for performing database operations without decrypting the data, thus maintaining security and privacy.

### Benefits and Considerations

- **Data Privacy and Security**: Ensures that sensitive data, such as personal information or proprietary corporate data, is stored securely, even in environments not directly controlled by your organization.
- **Searchability and Operational Efficiency**: Unlike other forms of encryption, deterministic encryption allows for efficient database operations such as indexing, searching, and grouping on encrypted data.
- **Compliance with Regulations**: Meets the requirements of regulations like GDPR, which mandate the protection of personal data, especially when processed or stored on external or cloud-based systems.

### When to Use

- **GDPR Compliance in Cloud-Based Storage**: Ideal for scenarios requiring GDPR compliance while using cloud-based databases.
- **Maintaining Operational Capabilities**: When the ability to search or group data directly in the database is necessary for operational efficiency.

### Important Considerations

- **Not a One-Size-Fits-All Solution**: Deterministic encryption is a tool among many in the security engineer's toolkit and should be used judiciously. It is not always the preferred method of encryption but can be effective for specific use cases.
- **Secure Key Management**: The security of deterministic encryption heavily relies on how the encryption keys are managed. It is crucial to ensure these keys are stored and handled securely, separate from the data they encrypt.

Deterministic encryption provides a balance between operational functionality and data security, making it a valuable option for specific use cases in cloud-based applications and services, particularly where GDPR compliance is a concern.

```c#
// Sample to demonstrate GDPR-compliant encryption of sensitive data using deterministic encryption
// for storage in a third-party hosted SQL server.

// Define the company name to be encrypted.
string companyName = "Undefined Corp";

// Create an instance of the symmetric encryption service with a secure password and salt.
// Note: In a production environment, securely manage the password and salt, avoiding hardcoded values.
var encryptionService = new Walter.Cypher.DeterministicEncryption(
    password: "My $ectet Pa$w0rd",
    salt: "123456789+*ç%&/"
);

// Encrypt the company name into a byte array.
byte[] encryptedBytes = encryptionService.Encrypt(companyName.ToBytes());

// Prepare the T-SQL command for data insertion, using the encrypted company name.
var tsql = @$"
declare @UndefinedCorp Varbinary(64) = {encryptedBytes.ToSqlBinaryString()};
declare @checksum int = CHECKSUM(@UndefinedCorp);

// Check for the existence of the company and insert if not present.
if not exists(select * from [dbo].[Companies] where [CompanyName] = @UndefinedCorp and [cs_CompanyName] = @checksum)
BEGIN
    INSERT [dbo].[Companies] ([CompanyName],[cs_CompanyName],[TrueUpDays],[AutoInvoice],[ApplicableLicenseExcempt])
    Values(@UndefinedCorp, @checksum, -1, 0, 1);
END
";

// Execute the T-SQL command to store the encrypted data.
using var con = new SqlConnection(config.GetConnectionString("Billing"));
using var cmd = con.CreateCommand();
cmd.CommandText = tsql;
cmd.CommandType = System.Data.CommandType.Text;
con.Open();
cmd.ExecuteNonQuery();

```


#### Sample Test Cases for Understanding the Symmetric Encryption

We've included sample test cases in our codebase to demonstrate the functionality of the Symmetric Encryption process. 
These samples are crafted not just to test the code, but also to serve as practical examples for those looking to understand the encryption and decryption mechanisms in depth.

#### Why Test Cases?

- **Hands-On Learning**: By setting breakpoints and stepping through these tests, you can gain a hands-on understanding of how the encryption and decryption process works.
- **Debugging and Inspection**: It's an excellent opportunity to inspect the flow of data, observe how the encryption algorithm behaves, and understand how different components interact.
- **Real-World Examples**: These tests are more than theoretical scenarios; they represent real-world use cases, helping you relate the functionality to practical applications.

#### What's in the Sample?

- **Encryption Consistency Test**: `Encrypt_WithSamePassword_ShouldGenerateDifferentCiphertexts` ensures that the encryption process is secure and generates different ciphertexts for the same plaintext.
- **Cross-Instance Compatibility Test**: `EncryptAndDecrypt_WithDifferentInstances_ShouldBeCompatible` confirms that the encrypted data by one instance can be decrypted by another, ensuring consistency across different instances.

#### How to Use the Sample


Understood, let's adjust the instructions to focus on using the NuGet package directly without the need for cloning a repository. Here's the revised section for your README.md:

markdown
Copy code
## How to Use the Sample

To effectively use and understand the Symmetric Encryption examples in the 'walter' NuGet package, follow these steps:

1. **Install the 'walter' NuGet Package**: Start by adding the 'walter' package to your C# project. This package is essential as it contains the components you'll need for the encryption examples.
2. **Navigate to your Tests**: Copy and past the test cases in the project's test project.
3. **Set Breakpoints**: Place breakpoints at critical points in the tests.
4. **Debug and Step Through**: Run the tests in debug mode and step through the code to observe how the encryption process is executed and validated.

We encourage you to explore these tests to deepen your understanding of symmetric encryption in a .NET environment.


```c#
[TestClass]
public class SymmetricEncryptionTests
{
    // This test verifies that the same text encrypted with the same password generates different byte arrays.
    // This is important to ensure that the encryption algorithm uses a unique initialization vector (IV) for each encryption,
    // which enhances security by producing different ciphertexts for the same plaintext.
    [TestMethod]
    public void Encrypt_WithSamePassword_ShouldGenerateDifferentCiphertexts()
    {
        var secretText = "Hello World";
        var encryptionInstance1 = new SymmetricEncryption("TestPassword");
        var encryptionInstance2 = new SymmetricEncryption("TestPassword");

        byte[] encryptedBytes1 = encryptionInstance1.Encrypt(Encoding.UTF8.GetBytes(secretText));
        byte[] encryptedBytes2 = encryptionInstance2.Encrypt(Encoding.UTF8.GetBytes(secretText));

        string ciphertext1 = Encoding.UTF8.GetString(encryptedBytes1);
        string ciphertext2 = Encoding.UTF8.GetString(encryptedBytes2);

        Assert.AreNotEqual(ciphertext1, ciphertext2, "Encrypted bytes should be different for the same input text.");

        string decryptedText1 = Encoding.UTF8.GetString(encryptionInstance1.Decrypt(encryptedBytes1));
        string decryptedText2 = Encoding.UTF8.GetString(encryptionInstance2.Decrypt(encryptedBytes2));

        Assert.AreEqual(decryptedText1, decryptedText2, "Decrypted texts should match the original secret text.");
    }

    // This test ensures that text encrypted by one instance of the SymmetricEncryption class
    // can be decrypted by another instance using the same password. This is crucial for verifying
    // that the encryption and decryption processes are compatible and consistent across different instances.
    [TestMethod]
    public void EncryptAndDecrypt_WithDifferentInstances_ShouldBeCompatible()
    {
        var secretText = "Hello World";
        var encryptionInstanceClient = new SymmetricEncryption("TestPassword");
        var encryptionInstanceServer = new SymmetricEncryption("TestPassword");

        string ciphertext = encryptionInstanceClient.EncryptString(secretText);
        string decryptedText = encryptionInstanceServer.DecryptString(ciphertext);

        Assert.AreEqual(secretText, decryptedText, "Decrypted text should match the original secret text.");
    }
}
```

### Extension methods
There are several extension methods that are usfull like ToBytes() and ToSqlBinaryString() as shown in the bellow code sample.

In the sample bellow we show how you could pre-populat database defaults in a GDPR compliant way where you can use the framweork to 
generate predefined standard values. 
```c#
var corp = "Undefined Corp";
var cypher = new Walter.Cypher.SymmetricEncryption(password: "My $ectet Pa$w0rd"
                                             , padding: System.Security.Cryptography.PaddingMode.PKCS7
                                             );

byte[] bytes = cypher.Encrypt(corp.ToBytes());
var tsql = @$"
    declare @UndefinedCorp Varbinary(64) = {bytes.ToSqlBinaryString()};
    declare @checksum int = CHECKSUM(@UndefinedCorp);

    if not exists(select * from  [dbo].[Companies] where [CompanyName] =@UndefinedCorp and [cs_CompanyName]= @checksum)
    BEGIN
        INSERT [dbo].[Companies] ([CompanyName],[cs_CompanyName],[TrueUpDays],[AutoInvoice])
        Values(@UndefinedCorp,@checksum,-1,0);
    END
";
using var con = new SqlConnection(config.GetConnectionString("Billing"));
using var cmd = con.CreateCommand();
cmd.CommandText = tsql;
cmd.CommandType = System.Data.CommandType.Text;
con.Open();
cmd.ExecuteNonQuery();
```

### AoT Exceptions Extension for .NET

The `ExceptionsExtension` class is a powerful utility for .NET developers, enhancing exception handling with additional diagnostic information. 
This extension provides methods to extract class names, method names, file names, and approximate line numbers from exceptions especially usefull in AoT. 

#### Features
- **ClassName**: Retrieves the class name where the exception originated.
- **MethodName**: Obtains the method name that generated the exception.
- **FileName**: Gets the filename of the class that generated the exception.
- **CodeLineNumber**: Provides the actual or approximate line number where the exception was thrown.

#### Usage

Here is a examples of how to use the `ExceptionsExtension` method:
```c#
try
{
    _ = File.Open("A:\\doesNotExist.txt", FileMode.Open);
}
catch (Exception e)
{
    //if the binary is AoT compiled the line number is this line
    _logger.LogError(e, "{Class}.{method} (line {line}) failed with a {exception}:{message}",e.ClassName(), e.MethodName(), e.CodeLineNumber(),e.GetType().Name,e.Message);
}

```



Visit www.asp-waf.com for more information.
			
