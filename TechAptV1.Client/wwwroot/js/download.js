/*function downloadFile(fileName, fileUrl) {
    var a = document.createElement('a');
    a.href = fileUrl;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
}
*/
window.downloadFileStream = async (fileName, contentType, streamRef) => {
    const fileStream = await streamRef.stream();
    const data = await new Response(fileStream).blob();

    const url = window.URL.createObjectURL(data);
    const a = document.createElement("a");
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
};
