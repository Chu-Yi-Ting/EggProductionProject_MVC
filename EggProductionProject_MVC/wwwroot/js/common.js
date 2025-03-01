﻿
//時間格式轉化
function formatDateTime(data) {
    if (data) {
        var date = new Date(data);
        var year = date.getFullYear();
        var month = ('0' + (date.getMonth() + 1)).slice(-2);
        var day = ('0' + date.getDate()).slice(-2);
        var hours = ('0' + date.getHours()).slice(-2);
        var minutes = ('0' + date.getMinutes()).slice(-2);
        var seconds = ('0' + date.getSeconds()).slice(-2);
        return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
    }
    return '';
}

//select 樣式美化(需要就貼去自己view程式裡面)
//document.addEventListener('DOMContentLoaded', function () {
//    const elements = document.querySelectorAll('.select-n');
//    elements.forEach(function (element) {
//        new Choices(element, {
//            searchEnabled: false,
//            itemSelectText: ''
//        });
//    });
//});