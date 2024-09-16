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

function showToast(message, toastClass) {
    const toastElement = document.getElementById('dynamicToast');
    const toastTitle = document.getElementById('toastTitle');
    const toastMessage = document.getElementById('toastMessage');

    // 根據通知狀態設置樣式和內容
    switch (toastClass) {
        case 'success':
            toastTitle.textContent = '新增成功';
            toastMessage.textContent = message;
            toastElement.classList.remove('bg-danger', 'bg-warning');
            toastElement.classList.add('bg-success');
            break;
        case 'fail':
            toastTitle.textContent = '新增失敗';
            toastMessage.textContent = message;
            toastElement.classList.remove('bg-success', 'bg-warning');
            toastElement.classList.add('bg-danger');
            break;
        case 'error':
            toastTitle.textContent = '錯誤';
            toastMessage.textContent = message || '系統錯誤，請稍後再試。';
            toastElement.classList.remove('bg-success', 'bg-danger');
            toastElement.classList.add('bg-warning');
            break;
        default:
            console.error('未知的通知狀態');
            return;
    }

    // 使用 Bootstrap 的 JavaScript API 來顯示 Toast
    const toast = new bootstrap.Toast(toastElement);
    toast.show();
}