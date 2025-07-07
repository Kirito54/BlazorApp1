(function(){
    const ui = document.getElementById('blazor-error-ui');
    if(!ui) return;
    window.addEventListener('offline', ()=>{
        ui.style.display = 'block';
        ui.querySelector('.reload').style.display='none';
        ui.firstChild.textContent = 'Потеряно соединение. Попытка переподключения...';
    });
    window.addEventListener('online', ()=>{
        ui.style.display = 'none';
        ui.querySelector('.reload').style.display='inline';
    });
})();
