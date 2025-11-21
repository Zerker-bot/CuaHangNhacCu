$(document).ready(function () {

    function formatCurrency(number) {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(number).replace('₫', 'VNĐ');
    }

    function updateCartSummary() {
        let total = 0;
        let itemsSelected = 0;
        let totalItems = $('.select-item').length;

        $('.select-item:checked').each(function () {
            total += parseFloat($(this).data('price'));
            itemsSelected++;
        });

        $('#selected-total').text(formatCurrency(total));
        $('.total-price-group span.small').text(`Tổng thanh toán (${itemsSelected} sản phẩm):`);
        $('#btn-checkout').prop('disabled', itemsSelected === 0);

        if (itemsSelected === 0) {
            $('#select-all').prop('checked', false);
            $('#select-all').prop('indeterminate', false);
        } else if (itemsSelected === totalItems) {
            $('#select-all').prop('checked', true);
            $('#select-all').prop('indeterminate', false);
        } else {
            $('#select-all').prop('checked', false);
            $('#select-all').prop('indeterminate', true);
        }
    }

    // Sự kiện Checkbox
    $('#select-all').on('change', function () {
        var isChecked = $(this).prop('checked');
        $('.select-item').prop('checked', isChecked);
        updateCartSummary();
    });

    $('.select-item').on('change', function () {
        updateCartSummary();
    });

    // AJAX Cập nhật số lượng
    $('.quantity-btn').on('click', function (e) {
        e.preventDefault();
        var button = $(this);
        var type = button.data('type');
        var itemId = button.data('item-id');
        var input = $(`.quantity-input[data-item-id=${itemId}]`);
        var currentVal = parseInt(input.val());

        if (type === 'plus') currentVal++;
        else if (type === 'minus' && currentVal > 1) currentVal--;
        else return;

        input.val(currentVal);
        updateQuantityOnServer(itemId, currentVal);
    });

    function updateQuantityOnServer(cartItemId, newQuantity) {
        var token = $('input[name="__RequestVerificationToken"]').val();
        fetch('/Cart/UpdateQuantity', {
            method: 'POST',
            headers: { 'RequestVerificationToken': token, 'Content-Type': 'application/x-www-form-urlencoded' },
            body: `cartItemId=${cartItemId}&newQuantity=${newQuantity}`
        })
            .then(res => res.ok ? res.json() : Promise.reject(res))
            .then(data => {
                if (data.success) {
                    var row = $(`#row-${cartItemId}`);
                    row.find('.item-total-price').text(data.newItemTotalFormatted);
                    row.find('.select-item').data('price', data.newItemTotal);
                    updateCartSummary();
                }
            })
            .catch(err => console.error(err));
    }

    window.addEventListener('pageshow', function (event) {
        updateCartSummary();
    });

    updateCartSummary();
});

function confirmRemove(e, url) {
    e.preventDefault();
    Swal.fire({
        title: "Xóa sản phẩm?",
        text: "Bạn có chắc chắn muốn xóa sản phẩm này khỏi giỏ hàng?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Xóa",
        cancelButtonText: "Hủy",
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6"
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = url;
        }
    });
}