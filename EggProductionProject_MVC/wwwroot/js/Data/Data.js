async function loadHouse() {
    const ChickHouseSelected = document.getElementById('ChickHouse_Selected');
    try {
        // 發送 GET 請求到後端 API
        const response = await fetch(`/Frontstage/DataApi/Get_House`, {
            method: 'GET',
        });

        if (!response.ok) {
            alert(`HTTP error! status: ${response.status}`);
            return;
        }

        const data = await response.json();// 將返回的數據解析為 JSON 格式
        ChickHouseSelected.innerHTML = ''; // 清空選單內容
        let options = '';// 遍歷數據並創建選項

        data.forEach(item => {
            options += `<option value="${item.value}">${item.text}</option>`;
        });

        // 更新下拉選單內容
        ChickHouseSelected.innerHTML = options;
    } catch (error) {
        alert('加載選單時發生錯誤:', error);
    }
}

async function loadArea() {
    const AreaSelected = document.getElementById('Area_Selected');
    //console.log(1);
    try {
        // 發送 GET 請求到後端 API
        const response = await fetch(`/Frontstage/DataApi/Get_Area`, {
            method: 'GET',
        });

        if (!response.ok) {
            alert(`HTTP error! status: ${response.status}`);
            return;
        }

        const data = await response.json();// 將返回的數據解析為 JSON 格式
        AreaSelected.innerHTML = ''; // 清空選單內容
        let options = '';// 遍歷數據並創建選項

        data.forEach(item => {
            options += `<option value="${item.value}">${item.text}</option>`;
        });
        // 更新下拉選單內容
        AreaSelected.innerHTML = options;
    } catch (error) {
        alert('加載選單時發生錯誤:', error);
    }
}