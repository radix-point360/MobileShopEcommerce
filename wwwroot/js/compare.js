
function addToCompare(productId) {
    let compareList = JSON.parse(localStorage.getItem('compareProducts')) || [];
    if (compareList.includes(productId)) {
        alert('Product already added for comparison.');
        return;
    }
    if (compareList.length >= 4) {
        alert('Maximum 4 products allowed for comparison.');
        return;
    }
    compareList.push(productId);
    localStorage.setItem('compareProducts', JSON.stringify(compareList));
    updateCompareBar();
    // alert('Product added to compare list.');;
}

function removeFromCompare(productId) {
    let compareList = JSON.parse(localStorage.getItem('compareProducts')) || [];
    compareList = compareList.filter(id => id !== productId);
    localStorage.setItem('compareProducts', JSON.stringify(compareList));
    updateCompareBar();
}

function removeCompareAndReload(productId) {
    removeFromCompare(productId);
    let compareList = JSON.parse(localStorage.getItem('compareProducts')) || [];
    if (compareList.length < 2) {
        window.location.href = '/Products';
        return;
    }
    const query = compareList.map(id => `ids=${id}`).join('&');
    window.location.href = `/Products/Compare?${query}`;
}

function updateCompareBar() {
    let compareList = JSON.parse(localStorage.getItem('compareProducts')) || [];
    const compareCount = document.getElementById('compareCount');
    const compareBar = document.getElementById('compareBar');
    if (compareCount) {
        compareCount.innerText = compareList.length;
    }
    if (compareBar) {
        compareBar.style.display = compareList.length > 0 ? 'flex' : 'none';
    }
}

function goToCompare() {
    let compareList = JSON.parse(localStorage.getItem('compareProducts')) || [];
    if (compareList.length < 2) {
        alert('Please add at least 2 products to compare.');
        return;
    }
    const query = compareList.map(id => `ids=${id}`).join('&');
    window.location.href = `/Products/Compare?${query}`;
}

function clearCompareItems() {
    localStorage.removeItem('compareProducts');
    updateCompareBar();
    window.location.href = '/Products';
}
document.addEventListener('DOMContentLoaded', function () {
    updateCompareBar();
});
