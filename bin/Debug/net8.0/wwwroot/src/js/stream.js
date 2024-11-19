    // API endpoint URL for fetching the file list
    const apiEndpoint = '@ip/clownwire/stream/links'; // Your actual API URL, it will be adjusted accordingly

    // Function to fetch the list of files from the API
    const fetchList = () => {
        return fetch(apiEndpoint)
            .then(response => {
                // Check if the response is OK (status 200-299)
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json(); // Parse the response as JSON
            })
            .then(files => {
                return files; // Return the list of files
            })
            .catch(error => {
                console.error('Error fetching files:', error);
                return []; // Return an empty list in case of error
            });
    };

    // Function to render list items
const renderList = (items) => {
const listContainer = document.getElementById('listContainer');
listContainer.innerHTML = ''; // Clear existing list items
items.forEach(item => {
    const div = document.createElement('div');
    div.classList.add('list-item');
    div.textContent = item.fileName; // Display the file name
    div.addEventListener('click', () => copyToClipboard(item.fileId)); // Copy file size to clipboard
    listContainer.appendChild(div);
});
};

// Function to copy text to clipboard (updated to copy file size)
const copyToClipboard = (fileId) => {
const textArea = document.createElement('textarea');
textArea.value = `@ip/clownwire/stream/${encodeURIComponent(fileId)}`; // Set the file size (length) to be copied
document.body.appendChild(textArea);
textArea.select();
document.execCommand('copy');
document.body.removeChild(textArea);
alert('Copied to clipboard: ' + fileId); // Alert showing the copied file size
};

    /*
    // Function to copy text to clipboard
    const copyToClipboard = (text) => {
        const textArea = document.createElement('textarea');
        textArea.value = text;
        document.body.appendChild(textArea);
        textArea.select();
        document.execCommand('copy');
        document.body.removeChild(textArea);
        alert('Copied to clipboard: ' + text);
    };

    // Function to render list items
    const renderList = (items) => {
        const listContainer = document.getElementById('listContainer');
        listContainer.innerHTML = ''; // Clear existing list items
        items.forEach(item => {
            const div = document.createElement('div');
            div.classList.add('list-item');
            div.textContent = item.fileName; // Display the file name
            div.addEventListener('click', () => copyToClipboard(item.fileName)); // Copy file name to clipboard
            listContainer.appendChild(div);
        });
    };
    */

    // Function to filter and sort the list based on search query
    const filterAndSortList = (items, query) => {
        return items
            .filter(item => item.fileName.toLowerCase().includes(query.toLowerCase())) // Filter by file name
            .sort((a, b) => a.fileName.localeCompare(b.fileName)); // Sort alphabetically
    };

    // Event listener for the search input
    const searchInput = document.getElementById('searchInput');
    searchInput.addEventListener('input', () => {
        const query = searchInput.value;
        const filteredAndSortedItems = filterAndSortList(fileData, query); // Update fileData based on search query
        renderList(filteredAndSortedItems);
    });

    // Initialize the list when the page loads
    let fileData = [];
    fetchList().then(items => {
        fileData = items; // Store the fetched file data in the fileData variable
        renderList(fileData); // Render the list initially
    });