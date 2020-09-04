## Null Value Possible
In some cases you may add a field to a type which already has Entities created previously. In this case the old data doesn't have a value for the field. 
If this happens, the field will return `null`, so you may need to catch this special exception in your code. 
