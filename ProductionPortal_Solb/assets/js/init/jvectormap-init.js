"use strict";
$(document).ready(function(){

    /* -----  jVector Map - World Map  ----- */
    $('#xp-world-map-markers').vectorMap({map: 'world_mill_en',backgroundColor: 'transparent',
        regionStyle: {
            initial: {
                fill: '#a6173f'
            }
    }});

    /* -----  jVector Map - USA Map  ----- */
    $('#xp-usa').vectorMap({map: 'us_aea_en',backgroundColor: 'transparent',
        regionStyle: {
            initial: {
                fill: '#a6173f'
            }
    }});

    /* -----  jVector Map - India Map  ----- */
    $('#xp-india').vectorMap({map: 'in_mill',backgroundColor: 'transparent',
        regionStyle: {
            initial: {
                fill: '#a6173f'
            }
    }});

    /* -----  jVector Map - Australia Map  ----- */
    $('#xp-australia').vectorMap({map : 'au_mill',backgroundColor : 'transparent',
        regionStyle : {
            initial : {
                fill : '#a6173f'
            }
    }});

    /* -----  jVector Map - France Map  ----- */
    $('#xp-france').vectorMap({map: 'fr_regions_mill',backgroundColor: 'transparent',
        regionStyle: {
            initial: {
                fill: '#a6173f'
            }
    }});

});