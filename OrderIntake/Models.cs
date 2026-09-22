using System;
using System.Collections.Generic;

public class Order
{
    public string OrderId { get; set; }

    public string PatientId { get; set; }

    public string SpecimenId { get; set; }

    public string SpecimenType { get; set; }

    public string Priority { get; set; }

    public string CollectionDate { get; set; }

    public List<string> RequestedTests { get; set; }
}

public class ValidationError
{
    public string Field { get; set; }

    public string Code { get; set; }

    public string Message { get; set; }
}

public class OrderResult
{
    public string Status { get; set; }

    public Order Order { get; set; }

    public List<ValidationError> Errors { get; set; }
}