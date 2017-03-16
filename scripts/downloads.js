$(function() {
    $.getJSON("https://api.github.com/repos/MathewSachin/Captura/releases").done(function(json) {
        for (var i = 0; i < json.length; ++i) {
            var release = json[i];
            
            var card = $("<div>");
            card.addClass("card");

            var header = $("<div>");
            header.addClass("card-header");
            header.text(release.name);
            card.append(header);
            
            var dls = $("<ul>");
            dls.addClass("list-group list-group-flush");

            for (var j = 0; j < release.assets.length; ++j)
            {
                var asset = release.assets[j];
                var size = Math.round(asset.size / 1024);

                var dl = $("<li>");
                dl.addClass("list-group-item");
                
                var dlLink = $("<a>");
                dlLink.attr("href", asset.browser_download_url);
                dlLink.text(asset.name + " (" + size + " KB)");
                
                dl.append(dlLink);
                dls.append(dl);
            }

            card.append(dls);

            // Zipball
            var zipball = $("<li>");
            zipball.addClass("list-group-item");

            var zipLink = $("<a>");
            zipLink.attr("href", release.zipball_url);
            zipLink.text("Source Code (zip)");

            zipball.append(zipLink);
            dls.append(zipball);

            // Tarball
            var tarball = $("<li>");
            tarball.addClass("list-group-item");

            var tarLink = $("<a>");
            tarLink.attr("href", release.tarball_url);
            tarLink.text("Source Code (tar.gz)");

            tarball.append(tarLink);
            dls.append(tarball);

            $(".download-list").append(card).append("<br>").append("<br>");
        }
    });
});