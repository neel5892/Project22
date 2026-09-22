# Order Intake Service

## Overview

This project validates laboratory order intake requests.

The service accepts JSON input and returns either:

- Accepted
- Rejected

along with validation errors.

---

## Processing Method

OrderResult Process(string json)

---

## Validation Rules

- orderId required
- orderId max length 20
- patientId required
- specimenId required

- specimenType must be:
  - blood
  - urine
  - tissue
  - saliva

- priority must be:
  - routine
  - urgent

- collectionDate must be valid
- collectionDate cannot be future date

- requestedTests must contain at least one item
- requestedTests cannot contain duplicates

- malformed JSON rejected

---

## Build

```bash
dotnet build