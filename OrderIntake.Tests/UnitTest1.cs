using Xunit;

public class OrderServiceTests
{
    [Fact]
    public void Valid_Order_Should_Be_Accepted()
    {
        var service = new OrderService();

        string json = """
        {
            "orderId":"ORD-1005",
            "patientId":"PAT-505",
            "specimenId":"SP-9005",
            "specimenType":"blood",
            "priority":"urgent",
            "collectionDate":"2025-09-18",
            "requestedTests":["Glucose"]
        }
        """;

        var result = service.Process(json);

        Assert.Equal("Accepted", result.Status);
    }

    [Fact]
    public void All_Errors_At_Once_Should_Be_Rejected()
    {
        var service = new OrderService();

        string json = """
        {
            "orderId":"",
            "patientId":"",
            "specimenId":"",
            "specimenType":"xyz",
            "priority":"bad",
            "collectionDate":"2099-01-01",
            "requestedTests":[]
        }
        """;

        var result = service.Process(json);

        Assert.Equal("Rejected", result.Status);
    }

    [Fact]
    public void OrderId_Length_Test()
    {
        var service = new OrderService();

        string json = """
        {
            "orderId":"123456789012345678901",
            "patientId":"PAT-505",
            "specimenId":"SP-9005",
            "specimenType":"blood",
            "priority":"urgent",
            "collectionDate":"2025-09-18",
            "requestedTests":["Glucose"]
        }
        """;

        var result = service.Process(json);

        Assert.Equal("Rejected", result.Status);
    }

    [Fact]
    public void Future_Date_Should_Be_Rejected()
    {
        var service = new OrderService();

        string json = """
        {
            "orderId":"ORD-1005",
            "patientId":"PAT-505",
            "specimenId":"SP-9005",
            "specimenType":"blood",
            "priority":"urgent",
            "collectionDate":"2099-01-01",
            "requestedTests":["Glucose"]
        }
        """;

        var result = service.Process(json);

        Assert.Equal("Rejected", result.Status);
    }

    [Fact]
    public void Duplicate_Requested_Tests_Should_Be_Rejected()
    {
        var service = new OrderService();

        string json = """
        {
            "orderId":"ORD-1005",
            "patientId":"PAT-505",
            "specimenId":"SP-9005",
            "specimenType":"blood",
            "priority":"urgent",
            "collectionDate":"2025-09-18",
            "requestedTests":["CBC","cbc"]
        }
        """;

        var result = service.Process(json);

        Assert.Equal("Rejected", result.Status);
    }

    [Fact]
    public void Empty_Requested_Tests_Should_Be_Rejected()
    {
        var service = new OrderService();

        string json = """
        {
            "orderId":"ORD-1005",
            "patientId":"PAT-505",
            "specimenId":"SP-9005",
            "specimenType":"blood",
            "priority":"urgent",
            "collectionDate":"2025-09-18",
            "requestedTests":[]
        }
        """;

        var result = service.Process(json);

        Assert.Equal("Rejected", result.Status);
    }

    [Fact]
    public void Invalid_SpecimenType_Should_Be_Rejected()
    {
        var service = new OrderService();

        string json = """
        {
            "orderId":"ORD-1005",
            "patientId":"PAT-505",
            "specimenId":"SP-9005",
            "specimenType":"xyz",
            "priority":"urgent",
            "collectionDate":"2025-09-18",
            "requestedTests":["Glucose"]
        }
        """;

        var result = service.Process(json);

        Assert.Equal("Rejected", result.Status);
    }

    [Fact]
    public void Malformed_Json_Should_Be_Rejected()
    {
        var service = new OrderService();

        string json = "{";

        var result = service.Process(json);

        Assert.Equal("Rejected", result.Status);
    }
}