# ccollab2eeplatform

Extract data from CodeCollaborator to EagleEye-Platform

## Usage

```sh
ccollab2eeplatform [options]
```  

### Options

| Option                 | Type      | Description                                                 |
| ---------------------- | --------- | ----------------------------------------------------------- |
| `--task-id` or `-t`    | string    | The `_id` property of the task which invoked this command.  |
| `--help` or `-h`       |           | Show helping message of this command.                       |

For example:

```sh
# Run this command with task id "57837029c66dc1a4570962b6"
ccollab2eeplatform --task-id="57837029c66dc1a4570962b6"

# Run this command with task id "57837029c66dc1a4570962b6"
ccollab2eeplatform -t "57837029c66dc1a4570962b6"

# Show `ccollab2eeplatform` helping message
ccollab2eeplatform -h

# Show `ccollab2eeplatform` helping message
ccollab2eeplatform --help
```

## Notes

1. in this version, the tool will wait a keyboard hit before exit.
2. the csv files file name will be displayed in the console window.
3. the csv file will be removed just before tool exit.

## Configuration Files

### `ConfigurationFiles\ccollab-cmd.json`

It's used for configuring code collaborator command line tool. For example:

```json
[
  {
    "Id": "Reviews",
    "CmdName": "ccollab.exe",
    "Args": "admin wget ",
    "ReviewsCreationDateLow": "2016-10-01",
    "ReviewsCreationDateHigh": "2016-10-02",
    "RelUrl": "..."
  },
  {
    "Id": "Defects",
    "CmdName": "ccollab.exe",
    "Args": "admin wget ",
    "ReviewsCreationDateLow": "2016-10-01",
    "ReviewsCreationDateHigh": "2016-10-02",
    "RelUrl": "..."
  }
]
```

You're able to modify `ReviewsCreationDateLow` and `ReviewsCreationDateHigh` pairs
to set a time range for extracting data from code collaborator.

Please don't modify other fields, because it requires the tweak and rebuild application.

### `ConfigurationFiles\employees.json`

It's used for configuring employee's login name, full name and his/her product name.

Only the review/defect records which are created by these configured employees will
be calculted.

For example:

```json
[
  {
    "LoginName": "pzhong",
    "FullName": "Patrick Zhong",
    "ProductName": "ViewPoint"
  }
]
```

`LoginName` is the id used for logging in code collaborator system.
`FullName` is user's full name, it will be shown on some types of chart.
`ProductName` is user's product name.

### `ConfigurationFiles\eagleeye-settings.json`

It's used for configuring charts to be generated. Fields details:

#### `ApiRootEndpoint`

EagleEye Platform API's root endpoint. For example:

```json
{ "ApiRootEndpoint": "http://localhost:3000/api/v1/" }
```

#### `Products`

Some of the charts are generated from specific products.

Like the "review count by month" charts collection.
We'll probably generate 2 charts:

1. Review count by month for FTView
2. Review count by month for ViewPoint

We'll use the products that is configured by this field.

```json
{ "Products": [ "FTView", "ViewPoint" ] }
```

Note: The product name here must be identical to the `ProductName` from employee.json.

#### `DefectInjectionStage`, `DefectSeverityTypes` and `DefectTypes`

It's used for configuring code collaborator specific lists.
The value must be identical with the value in code collaborator.

```json
{
  "DefectInjectionStage": [
    "Code/Unit Test",
    "Design",
    "Requirements",
    "Integration/Test"
  ],
  "DefectSeverityTypes": [
    "Major",
    "Minor"
  ],
  "DefectTypes": [
    "algorithm/logic",
    "build",
    "data access",
    "documentation",
    "initialization",
    "interface",
    "maintainability",
    "performance",
    "requirements/design",
    "robustness/error handling",
    "style",
    "testing"
  ]
}
```

#### Others

All the other fields are used for configuring chart ids.
Don't modify the existing keys like `CodeDefectDensityUploaded`.
Please update the `ChartId` to the real chart id in EagleEye Platform.

Chart collection `CommentDensityChangedByMonth` may contain multiple charts.
We're able to generate a chart for each product.

`"ProductName": "*"` means generated a chart for all of the products.

```json
{
  "CodeDefectDensityUploaded": {
    "ChartId": "5799641be24561202bc7190d"
  },
  "CommentDensityChangedByMonth": [
    {
      "ProductName": "*",
      "ChartId": "5809b2827e9254582e54116d"
    },
    {
      "ProductName": "ViewPoint",
      "ChartId": "5809b2827e9254582e54116d"
    },
    {
      "ProductName": "FTView",
      "ChartId": "5809b2827e9254582e54116d"
    },
    {
      "ProductName": "FTSP_FTAE",
      "ChartId": "5809b2827e9254582e54116d"
    }
  ]
}
```
