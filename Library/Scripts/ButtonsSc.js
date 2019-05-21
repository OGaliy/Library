$('input[type=file]').change(function (e) {
    $in = $(this);
    if ($in != null)
        $in.next().html($in.val().replace(/.*\\(.+)/, '$1'));
   
});