var log_box = document.getElementById("log");
var fileInput = document.getElementById('file'); 
var textInput = document.getElementById('text');
var upload = document.getElementById('upload'); 
var progressBar = document.getElementById('progressBar');
var progressBar2 = document.getElementById('progressBar2');
var download = document.getElementById('download');
var note = document.getElementById('note');
var dropArea = document.getElementById('dropArea');
var logBox = document.getElementById('log');
var dropped = [];



var porcent = 0;
var count; 
var length;

// This will be replaced dynamically with the correct API URL
const apiUrl = '@ip/clownwire/upload/file'; // Use your dynamic IP here





// Handle drag over, leave, and drop for the drop area
dropArea.addEventListener('dragover', function(event) {
    event.preventDefault();
    dropArea.classList.add('drag');
});

dropArea.addEventListener('dragleave', function() {
    dropArea.classList.remove('drag');
});

dropArea.addEventListener('drop', function(event) {
    event.preventDefault();
    dropArea.classList.remove('drag');
    const items = event.dataTransfer.items;
    processItems(items); // Process dropped files and directories
});


   // Function to process items (files and directories)
   function processItems(items) 
   {
        for (let i = 0; i < items.length; i++) {
            const entry = items[i].webkitGetAsEntry ? items[i].webkitGetAsEntry() : null;
            if (entry) {
                handleEntry(entry);
            }
        }
        
    }


       // Handle a directory or file entry
       function handleEntry(entry) 
       {
        if (entry.isDirectory) {
            // Handle directory: Create a directory object with its path and files
            const dirPath = entry.fullPath || entry.webkitRelativePath;
            const dirItem = { path: dirPath, files: [] };
            const reader = entry.createReader();
            reader.readEntries(entries => {
                entries.forEach(handleEntry);  // Recurse into subdirectories
            });
        } else if (entry.isFile) {
            // Handle file: Add it to the dropped array
            entry.file(function(file) {
                const fileItem = { name: file.name, size: file.size, path: file.webkitRelativePath || file.name };
                dropped.push(fileItem);
            });
        }
        }

// Function to handle file upload in chunks
async function uploadFileInChunks(file, text,was_dropped) 
{
    const chunkSize = 10 * 1024 * 1024; // 10 MB chunks
    const totalChunks = Math.ceil(file.size / chunkSize);

    progressBar.style.display = 'block'; // Ensure the progress bar is visible
    for (let chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++) {
        // Calculate percentage progress
        porcent = Math.round((chunkIndex + 1) / totalChunks * 100);
        progressBar.value = porcent;
        upload.textContent = `Uploading...: ${porcent}%`;
        //console.log(${porcent}%);
        const start = chunkIndex * chunkSize;
        const end = Math.min(start + chunkSize, file.size);
        const chunk = file.slice(start, end);

        const formData = new FormData();
        formData.append('fileChunk', chunk, was_dropped?file.webkitRelativePath:file.name); // Append the file chunk
        formData.append('text', text); // Append the associated text
        formData.append('chunkIndex', chunkIndex); // Append the current chunk index
        formData.append('totalChunks', totalChunks); // Append total number of chunks

        try {
            const response = await fetch(apiUrl, {
                method: 'POST',
                body: formData,
                headers: {
                'Accept': 'application/json',
                 // Ensure the correct response format
                }
            });

            if (!response.ok) {
                console.log(`Upload failed: ${response.statusText}`);
                throw new Error(`Upload failed: ${response.statusText}`);
            }

            const result = await response.json();
            console.log(result.message);
            console.log(result.message); // Log success message
        } catch (error) {
            console.log(`Error uploading chunk: ${error}`);
            console.error('Error uploading chunk:', error);
            break; // Stop uploading if there's an error
        }
    }

    // After the upload is complete
    upload.textContent = "Upload";
    progressBar.style.display = 'none';
    //progressBar2.style.display = 'none';
    progressBar.value = 0;
    count++;
    progressBar2.value = Math.round((count) / length* 100);
    console.log(`Global Operation: ${Math.round((count) / length* 100)}%`);

    if(count == length-1)
    {
        progressBar2.style.display = 'none';
        upload.textContent = "Done!!!";
        //setTimeout(()=>{upload.textContent = "Upload";},2000);
    }
    console.log("DONE");
}

// Handle the upload button click
upload.addEventListener('click', () => {

    length = fileInput.files.length;
    count = 0;
    //adding a general or global progress bar
    if(dropped.length > 0)
    {
        process_drop_upload(dropped);
        return;
    }
    if(length > 1)
        {    
            progressBar2.style.display = 'block'; // Ensure the progress bar is visible
        }
    for(let f = 0; f < length; f++)
    {

        try{
            

            let file = fileInput.files[f];
            let text = textInput.value || "not-given"; // Default to "not-given" if no text is entered
            
            if (file) 
            {
                upload.disabled = true;  // Disable the upload button during the upload
                download.disabled = true; 
                note.disabled = true; 
                upload.textContent = "Uploading...";
                log_box.textContent = "";  // Clear previous logs

                // Start the file upload process
                uploadFileInChunks(file, text,false).finally(() => 
                {
                    upload.disabled = false;  // Re-enable the button after upload completes
                    download.disabled = false; 
                    note.disabled = false; 

                    upload.textContent = "Upload";  // Reset the button text
                });
            } else {
                console.log('No file selected.');
                console.error('No file selected.');
            }
        }catch{

        }
    }
        //progressBar2.style.display = 'none';

});


function process_drop_upload(files)
{
    length = files.length;
    count = 0;
    //adding a general or global progress bar
    console.log(`Process Droped Files: ${files}`);
    if(length > 1)
        {    
            progressBar2.style.display = 'block'; // Ensure the progress bar is visible
        }
    for(let f = 0; f < length; f++)
    {

        try{
            

            let file = files[f];
            let text = textInput.value || "not-given"; // Default to "not-given" if no text is entered
            
            if (file) 
            {
                upload.disabled = true;  // Disable the upload button during the upload
                download.disabled = true; 
                note.disabled = true; 
                upload.textContent = "Uploading...";
                log_box.textContent = "";  // Clear previous logs

                // Start the file upload process
                uploadFileInChunks(file, text,true).finally(() => 
                {
                    upload.disabled = false;  // Re-enable the button after upload completes
                    download.disabled = false; 
                    note.disabled = false; 
                    dropped = [];
                    upload.textContent = "Upload";  // Reset the button text
                });
            } else {
                console.log('No file selected.');
                console.error('No file selected.');
            }
        }catch{
                console.log("Something went wrong...");
        }
    }
      
}



// Function to prepare the tree structure and upload it
function prepareAndUploadFileTree() {
    // Sort the dropped files into a hierarchical directory structure
    let fileTree = buildFileTree(dropped);

    // Prepare the request payload
    let formData = new FormData();
    formData.append('fileTree', JSON.stringify(fileTree));
    formData.append('text', textInput.value || "not-given");

    // Start the upload process
    uploadFileTree(formData);
}

// Build a hierarchical file tree from the dropped files
function buildFileTree(files) {
    let fileTree = {};

    files.forEach(file => {
        const pathParts = file.path.split('/');
        let currentDir = fileTree;

        // Loop through the path parts (directories)
        pathParts.forEach((part, index) => {
            if (index === pathParts.length - 1) {
                // We're at the file, add it to the current directory
                currentDir.files = currentDir.files || [];
                currentDir.files.push({ name: part, size: file.size });
            } else {
                // We're in a directory, make sure it exists in the tree
                currentDir[part] = currentDir[part] || {};
                currentDir = currentDir[part];
            }
        });
    });

    return fileTree;
}
// Function to upload the entire file tree to the server
async function uploadFileTree(formData) {
    try {
        const response = await fetch(apiUrl, {
            method: 'POST',
            body: formData,
            headers: {
                'Accept': 'application/json',
            }
        });

        if (response.ok) {
            const result = await response.json();
            console.log('Upload successful:', result);
            log_box.textContent = "Upload complete!";
        } else {
            console.error('Upload failed:', response.statusText);
            log_box.textContent = `Upload failed: ${response.statusText}`;
        }
    } catch (error) {
        console.error('Error uploading file tree:', error);
        log_box.textContent = "Error uploading file tree.";
    }
}