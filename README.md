# BatchProcessing
Batch processing of anonymous tasks. 
This batch manager can be used for sequential and parallel operation, depending on the settings. Examples of use: zip archives, math calculations and others.

## Examples
```CSharp
public Run(IBatchManager batchManager)
{
    var actions = new Dictionary<Guid, Action>();

    actions.Add(Guid.NewGuid(), () => { Task.Delay(1000); });

    var jobId = batchManager.RunProcessing(actions);
}
```

## Possible implementation
If your application has a need to store state in the database, you can use the example of a [scheme](https://github.com/barkovmihail/BatchProcessing/blob/main/AppData/table-schemas/batch-schema.sql) databases for Transact-SQL


### License
[MIT licensed](./LICENSE).
