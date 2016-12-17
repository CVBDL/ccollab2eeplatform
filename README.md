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
