        // The 'filesFromServer' array will be populated dynamically from the server
        let filesFromServer = [];
        var upload = document.getElementById('upload');
        var note = document.getElementById('note');


        async function downloadFile(fileName) {
            const apiUrl = `@ip/clownwire/download/${fileName}`; // The endpoint URL
            const url = apiUrl;
            try {
                // Fetch the file from the server
                const response = await fetch(url);

                if (!response.ok) {
                    // Handle failed request
                    throw new Error(`Failed to fetch file: ${response.statusText}`);
                }

                // Get the file as a blob
                const blob = await response.blob();
                
                // Create a URL for the blob and create an anchor element to trigger the download
                const downloadUrl = window.URL.createObjectURL(blob);
                const link = document.createElement('a');
                link.href = downloadUrl;
                link.download = fileName; // Name the file as it is on the server
                
                // Trigger the download
                link.click();

                // Clean up the object URL after download
                setTimeout(() => {
                    window.URL.revokeObjectURL(downloadUrl);
                }, 100); // 100ms delay is generally enough
            } catch (error) {
                console.error('Error downloading file:', error);
            }
        }

        async function fetchFileList() {
    try {
        const response = await fetch('@ip/clownwire/download/filelist'); // Adjust URL to match your API

        if (!response.ok) {
            throw new Error('Failed to fetch file list');
        }

        // Log the raw response text to check the response format
        const responseText = await response.text();  // Read the response as text first
        console.log("Response Text: ", responseText);

        // Now try parsing the JSON
        const filesFromServer = JSON.parse(responseText);
        
        // Check if the filesFromServer is an array
        console.log("Files from server:", filesFromServer);

        // Dynamically populate the file list
        const fileListDiv = document.getElementById('fileList');
        fileListDiv.innerHTML = ''; // Clear any existing content

        // Loop through the array and create the UI dynamically
        filesFromServer.forEach((file) => {
            const fileItem = document.createElement('label');
            fileItem.innerHTML = `
                <input type="checkbox" name="file" value="${file.FileName}" class="file-checkbox">
                ${file.FileName} (${file.FileSize})
            `;
            fileListDiv.appendChild(fileItem);
        });

        // Re-bind the checkbox change event after the list is populated
        bindCheckboxEvents();

    } catch (error) {
        console.error('Error fetching file list:', error);
        log('Failed to load file list from the server.');
    }
}


        /*
        async function fetchFileList() {
    try {
        const response = await fetch('@ip/clownwire/download/filelist'); // Adjust URL to match your API

        if (!response.ok) {
            throw new Error('Failed to fetch file list');
        }

        // Log the raw response text to check the response format
        const responseText = await response.text();  // Read the response as text first
        console.log("Response Text: ", responseText);

        // Now try parsing the JSON
        const filesFromServer = JSON.parse(responseText);
        
        // Check if the filesFromServer is an array
        console.log("Files from server:", filesFromServer);

        // Dynamically populate the file list
        const fileListDiv = document.getElementById('fileList');
        fileListDiv.innerHTML = ''; // Clear any existing content

        filesFromServer.forEach((file) => {
            const fileItem = document.createElement('label');
            fileItem.innerHTML = `
                <input type="checkbox" name="file" value="${file.fileName}" class="file-checkbox">
                ${file.fileName} (${file.fileSize})
            `;
            fileListDiv.appendChild(fileItem);
        });

        // Re-bind the checkbox change event after the list is populated
        bindCheckboxEvents();

    } catch (error) {
        console.error('Error fetching file list:', error);
        log('Failed to load file list from the server.');
    }
}

*/
        /*
        // This function will fetch the file list from the server
        async function fetchFileList() {
            try {
                const response = await fetch('@ip/clownwire/download/filelist'); // Adjust URL to match your API
                if (!response.ok) {
                    throw new Error('Failed to fetch file list');
                }

                // Parse the JSON response from the server
                filesFromServer = await response.json();

                // Dynamically populate the file list
                const fileListDiv = document.getElementById('fileList');
                fileListDiv.innerHTML = ''; // Clear any existing content

                filesFromServer.forEach((file) => {
                    const fileItem = document.createElement('label');
                    fileItem.innerHTML = `
                        <input type="checkbox" name="file" value="${file.fileName}" class="file-checkbox">
                        ${file.fileName} (${file.fileSize})
                    `;
                    fileListDiv.appendChild(fileItem);
                });

                // Re-bind the checkbox change event after the list is populated
                bindCheckboxEvents();

            } catch (error) {
                console.error('Error fetching file list:', error);
                log('Failed to load file list from the server.');
            }
        }
        */

        // Function to enable or disable the download button based on selected files
        function bindCheckboxEvents() {
            const checkboxes = document.querySelectorAll('.file-checkbox');
            checkboxes.forEach((checkbox) => {
                checkbox.addEventListener('change', () => {
                    const selectedFiles = Array.from(checkboxes).filter(checkbox => checkbox.checked);
                    downloadButton.disabled = selectedFiles.length === 0;
                    switch(downloadButton.disabled)
                    {
                        case true:
                            downloadButton.textContent = "Download Selected Files";
                            break;
                        case false:
                            downloadButton.textContent = `Download ${selectedFiles.length} File(s)`;
                            break;
                    }
                });
            });
        }

        // Add search functionality for filtering the file list
        const searchBar = document.getElementById('searchBar');
        searchBar.addEventListener('input', () => {
            const query = searchBar.value.toLowerCase();
            const filteredFiles = filesFromServer.filter(file => 
                file.fileName.toLowerCase().includes(query)
            );

            // Re-populate the file list with the filtered files
            const fileListDiv = document.getElementById('fileList');
            fileListDiv.innerHTML = ''; // Clear the current list

            filteredFiles.forEach((file) => {
                const fileItem = document.createElement('label');
                fileItem.innerHTML = `
                    <input type="checkbox" name="file" value="${file.fileName}" class="file-checkbox">
                    ${file.fileName} (${file.fileSize})
                `;
                fileListDiv.appendChild(fileItem);
            });

            // Re-bind the checkbox event after filtering
            bindCheckboxEvents();
        });

        // Select All checkbox functionality
        const selectAllCheckbox = document.getElementById('selectAllCheckbox');
        selectAllCheckbox.addEventListener('change', () => {
            const checkboxes = document.querySelectorAll('.file-checkbox');
            checkboxes.forEach((checkbox) => {
                checkbox.checked = selectAllCheckbox.checked;
            });

            // Update the download button text based on selection
            const selectedFiles = Array.from(checkboxes).filter(checkbox => checkbox.checked);
            downloadButton.disabled = selectedFiles.length === 0;
            if (selectedFiles.length > 0) {
                downloadButton.textContent = `Download ${selectedFiles.length} File(s)`;
            } else {
                downloadButton.textContent = "Download Selected Files";
            }
        });

        // Download the selected files
        const downloadButton = document.getElementById('download');
        downloadButton.addEventListener('click', async () => {
            downloadButton.disabled = true;
            upload.disabled = true;

            const selectedFiles = Array.from(document.querySelectorAll('.file-checkbox'))
                .filter(checkbox => checkbox.checked)
                .map(checkbox => checkbox.value);

            if (selectedFiles.length > 0) {
                console.log(`Downloading: ${selectedFiles.join(', ')}`);
                for (let item of selectedFiles) {
                    await downloadFile(item);
                }
            } else {
                log("No files selected.");
            }
        });

        // Log function to update the log box
        function log(text) {
            const logBox = document.getElementById("log");
            logBox.textContent = text + "\n" + logBox.textContent;
        }

        // Fetch the file list from the server when the page loads
        window.onload = fetchFileList;