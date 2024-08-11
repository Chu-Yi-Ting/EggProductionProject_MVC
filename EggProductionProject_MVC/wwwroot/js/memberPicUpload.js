function previewImage(inputFile, img, btn) {
    var file = inputFile.files[0];

    if (inputFile.files[0] != null) {
        var allowType = "image.*";
        if (file.type.match(allowType)) {
            //預覽
            var reader = new FileReader();
            reader.onload = function (e) {
                img.attr("src", e.target.result);
                img.attr("title", file.name);
            };
            reader.readAsDataURL(file);


            btn.prop("disabled", false);
        }
        else {
            alert("不允許的檔案類型");
            $("#btnUpdate").prop("disabled", true);
            $("#Picture").val("");
        }
    }

}