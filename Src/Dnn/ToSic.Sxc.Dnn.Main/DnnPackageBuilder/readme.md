# Additional files to build the DNN Package

This folder just contains things we would remove from the root folder, to be included in the dnn package. 

We are trying to clean up the main folder, so this is where add-on material should be placed

## Special Cleanup folder

This contains a txt file with folders that should be flushed completely on every upgrade. 

We need this, because the `dist/` folder can change often (compiled JavaScript) so we don't leave behind a bunch of unused JS. 