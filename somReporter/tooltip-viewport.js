$(document).ready(function () {
  $('.tooltip-right').tooltip({
      placement: 'right',
      html: 'true',
    viewport: {selector: 'body', padding: 2}
  })
  $('.tooltip-bottom').tooltip({
      placement: 'bottom',
      html: 'true',
    viewport: {selector: 'body', padding: 2}
  })
  $('.tooltip-viewport-right').tooltip({
      placement: 'right',
      html: 'true',
    viewport: {selector: '.container-viewport', padding: 2}
  })
  $('.tooltip-viewport-bottom').tooltip({
      placement: 'bottom',
      html: 'true',
      viewport: {selector: '.container-viewport', padding: 2}
  })
})
