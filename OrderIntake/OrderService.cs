using System.Text.Json;

public class OrderService
{
    public OrderResult Process(string json)
    {
        var errors = new List<ValidationError>();

        Order? order;

        try
        {
            order = JsonSerializer.Deserialize<Order>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch
        {
            return new OrderResult
            {
                Status = "Rejected",
                Errors = new List<ValidationError>
                {
                    new ValidationError
                    {
                        Field = "$",
                        Code = "MALFORMED_INPUT",
                        Message = "Invalid JSON"
                    }
                }
            };
        }

        if (order == null)
        {
            return new OrderResult
            {
                Status = "Rejected",
                Errors = new List<ValidationError>
                {
                    new ValidationError
                    {
                        Field = "$",
                        Code = "MALFORMED_INPUT",
                        Message = "Invalid JSON"
                    }
                }
            };
        }

        ValidateOrderId(order, errors);
        ValidatePatientId(order, errors);
        ValidateSpecimenId(order, errors);
        ValidateSpecimenType(order, errors);
        ValidatePriority(order, errors);
        ValidateCollectionDate(order, errors);
        ValidateRequestedTests(order, errors);

        if (errors.Count > 0)
        {
            return new OrderResult
            {
                Status = "Rejected",
                Errors = errors
            };
        }

        return new OrderResult
        {
            Status = "Accepted",
            Order = order,
            Errors = new List<ValidationError>()
        };
    }

    private void ValidateOrderId(Order order, List<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(order.OrderId))
        {
            errors.Add(new ValidationError
            {
                Field = "orderId",
                Code = "REQUIRED",
                Message = "OrderId is required"
            });
        }
        else if (order.OrderId.Length > 20)
        {
            errors.Add(new ValidationError
            {
                Field = "orderId",
                Code = "MAX_LENGTH",
                Message = "Maximum length is 20"
            });
        }
    }

    private void ValidatePatientId(Order order, List<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(order.PatientId))
        {
            errors.Add(new ValidationError
            {
                Field = "patientId",
                Code = "REQUIRED",
                Message = "PatientId is required"
            });
        }
    }

    private void ValidateSpecimenId(Order order, List<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(order.SpecimenId))
        {
            errors.Add(new ValidationError
            {
                Field = "specimenId",
                Code = "REQUIRED",
                Message = "SpecimenId is required"
            });
        }
    }

    private void ValidateSpecimenType(Order order, List<ValidationError> errors)
    {
        var validTypes = new[]
        {
            "blood",
            "urine",
            "tissue",
            "saliva"
        };

        if (!validTypes.Contains(order.SpecimenType?.ToLower()))
        {
            errors.Add(new ValidationError
            {
                Field = "specimenType",
                Code = "INVALID_VALUE",
                Message = "Invalid specimen type"
            });
        }
    }

    private void ValidatePriority(Order order, List<ValidationError> errors)
    {
        var validPriorities = new[]
        {
            "routine",
            "urgent"
        };

        if (!validPriorities.Contains(order.Priority?.ToLower()))
        {
            errors.Add(new ValidationError
            {
                Field = "priority",
                Code = "INVALID_VALUE",
                Message = "Invalid priority"
            });
        }
    }

    private void ValidateCollectionDate(Order order, List<ValidationError> errors)
    {
        if (!DateTime.TryParse(order.CollectionDate, out DateTime date))
        {
            errors.Add(new ValidationError
            {
                Field = "collectionDate",
                Code = "INVALID_DATE",
                Message = "Invalid date"
            });

            return;
        }

        if (date.Date > DateTime.Today)
        {
            errors.Add(new ValidationError
            {
                Field = "collectionDate",
                Code = "FUTURE_DATE",
                Message = "Date cannot be in future"
            });
        }
    }

    private void ValidateRequestedTests(Order order, List<ValidationError> errors)
    {
        if (order.RequestedTests == null ||
            order.RequestedTests.Count == 0)
        {
            errors.Add(new ValidationError
            {
                Field = "requestedTests",
                Code = "REQUIRED",
                Message = "At least one test required"
            });

            return;
        }

        var duplicates =
            order.RequestedTests
                 .GroupBy(x => x.ToLower())
                 .Any(g => g.Count() > 1);

        if (duplicates)
        {
            errors.Add(new ValidationError
            {
                Field = "requestedTests",
                Code = "DUPLICATE",
                Message = "Duplicate tests found"
            });
        }
    }
}