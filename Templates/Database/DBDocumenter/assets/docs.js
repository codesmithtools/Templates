/* DBDocumenter: the pages' script. The theme toggle, the sidebar drawer, copy buttons, the search (its index is
   assets/search-index.js, so it works from the file system), "on this page", and the diagram's pan and zoom. */
(function () {
  "use strict";

  var doc = document.documentElement;
  var body = document.body;
  var root = body.getAttribute("data-root") || "";
  var isMac = /Mac|iPhone|iPad/.test(navigator.platform || navigator.userAgent || "");

  function $(selector, scope) { return (scope || document).querySelector(selector); }
  function $$(selector, scope) { return Array.prototype.slice.call((scope || document).querySelectorAll(selector)); }
  function escapeHtml(text) {
    return String(text).replace(/[&<>"']/g, function (c) {
      return { "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[c];
    });
  }
  function isTyping(element) {
    return element && (element.isContentEditable || /^(INPUT|TEXTAREA|SELECT)$/.test(element.tagName));
  }

  var ICONS = {
    table: '<path d="M12 3v18"/><rect width="18" height="18" x="3" y="3" rx="2"/><path d="M3 9h18"/><path d="M3 15h18"/>',
    view: '<path d="M2.062 12.348a1 1 0 0 1 0-.696 10.75 10.75 0 0 1 19.876 0 1 1 0 0 1 0 .696 10.75 10.75 0 0 1-19.876 0"/><circle cx="12" cy="12" r="3"/>',
    procedure: '<rect width="18" height="18" x="3" y="3" rx="2"/><path d="M9 17c2 0 2.8-1 2.8-2.8V10c0-2 1-3.3 3.2-3"/><path d="M9 11.2h5.7"/>',
    column: '<line x1="4" x2="20" y1="9" y2="9"/><line x1="4" x2="20" y1="15" y2="15"/><line x1="10" x2="8" y1="3" y2="21"/><line x1="16" x2="14" y1="3" y2="21"/>',
    parameter: '<path d="M8 21s-4-3-4-9 4-9 4-9"/><path d="M16 3s4 3 4 9-4 9-4 9"/><line x1="15" x2="9" y1="9" y2="15"/><line x1="9" x2="15" y1="9" y2="15"/>',
    enter: '<path d="m9 10-5 5 5 5"/><path d="M20 4v7a4 4 0 0 1-4 4H4"/>',
    search: '<circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/>'
  };
  function icon(name, css) {
    return '<svg class="' + (css || "icon") + '" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">' + (ICONS[name] || "") + "</svg>";
  }

  /* Theme: the system's until the toggle picks one, which is remembered */

  var darkQuery = window.matchMedia ? window.matchMedia("(prefers-color-scheme: dark)") : null;
  function theme() { return doc.getAttribute("data-theme") || (darkQuery && darkQuery.matches ? "dark" : "light"); }
  function labelThemeButtons() {
    var next = theme() === "dark" ? "light" : "dark";
    $$("[data-theme-toggle]").forEach(function (button) {
      button.setAttribute("aria-label", "Switch to the " + next + " theme");
      button.title = "Switch to the " + next + " theme";
    });
  }
  $$("[data-theme-toggle]").forEach(function (button) {
    button.addEventListener("click", function () {
      var next = theme() === "dark" ? "light" : "dark";
      doc.setAttribute("data-theme", next);
      try { localStorage.setItem("dbdocs-theme", next); } catch (e) { /* storage off: this page only */ }
      labelThemeButtons();
    });
  });
  if (darkQuery && darkQuery.addEventListener) darkQuery.addEventListener("change", labelThemeButtons);
  labelThemeButtons();

  $$("[data-shortcut]").forEach(function (kbd) { kbd.textContent = isMac ? "⌘K" : "Ctrl K"; });

  /* The sidebar: a drawer on narrow screens; the current page's link scrolled into view */

  var sidebar = $("#sidebar");
  function setNav(open) {
    body.classList.toggle("nav-open", open);
    $$("[data-nav-open]").forEach(function (b) { b.setAttribute("aria-expanded", open ? "true" : "false"); });
    if (open) {
      var first = $(".nav-item.is-active, .nav-link.is-active", sidebar) || $(".search-field", sidebar);
      if (first) first.focus({ preventScroll: true });
    }
  }
  $$("[data-nav-open]").forEach(function (b) { b.addEventListener("click", function () { setNav(true); }); });
  $$("[data-nav-close]").forEach(function (b) { b.addEventListener("click", function () { setNav(false); }); });

  var nav = $(".nav");
  var activeItem = $(".nav-item.is-active");
  if (nav && activeItem) {
    var top = activeItem.offsetTop - nav.offsetTop;
    if (top > nav.clientHeight - 60) nav.scrollTop = top - nav.clientHeight / 2;
  }

  /* Copy buttons: a name (data-copy), or a code block's text (data-copy-code) */

  function copyText(text, button) {
    function done() {
      button.classList.add("is-copied");
      var label = $("span", button);
      var before = label ? label.textContent : null;
      if (label) label.textContent = "Copied";
      clearTimeout(button._copied);
      button._copied = setTimeout(function () {
        button.classList.remove("is-copied");
        if (label) label.textContent = before;
      }, 1600);
    }
    function fallback() {
      var area = document.createElement("textarea");
      area.value = text;
      area.setAttribute("readonly", "");
      area.style.position = "fixed";
      area.style.opacity = "0";
      body.appendChild(area);
      area.select();
      try { document.execCommand("copy"); done(); } catch (e) { /* nothing to copy with */ }
      body.removeChild(area);
    }
    if (navigator.clipboard && window.isSecureContext) navigator.clipboard.writeText(text).then(done, fallback);
    else fallback();
  }
  $$("[data-copy]").forEach(function (button) {
    button.addEventListener("click", function () { copyText(button.getAttribute("data-copy"), button); });
  });
  $$("[data-copy-code]").forEach(function (button) {
    button.addEventListener("click", function () {
      var lines = $$(".line", button.closest(".code-block"));
      copyText(lines.map(function (line) { return line.textContent; }).join("\n"), button);
    });
  });

  /* Search: every table, view, procedure, column and parameter */

  var palette = $("#search");
  var input = palette && $(".palette-input", palette);
  var results = palette && $(".palette-results", palette);
  var KINDS = ["table", "view", "procedure", "column", "parameter"];
  var GROUPS = { table: "Tables", view: "Views", procedure: "Procedures", column: "Columns", parameter: "Parameters" };
  var entries = (window.DBDOCS_INDEX || []).map(function (row) {
    return { kind: row[0], name: row[1], parent: row[2], url: row[3], detail: row[4], lower: row[1].toLowerCase(), parentLower: (row[2] || "").toLowerCase() };
  });
  var shown = [];
  var selected = 0;
  var lastFocus = null;

  // How well a name matches: whole, start, a word's start (order_id), inside, or (where fuzzy, when nothing else matches)
  // its letters in order close together (lnitm: LineItem).
  function score(name, query, fuzzy) {
    if (!query) return 1;
    if (name === query) return 100;
    if (name.indexOf(query) === 0) return 80 - Math.min(name.length - query.length, 20) * 0.5;
    var at = name.indexOf(query);
    if (at > 0) return (/[_\-\s.]/.test(name.charAt(at - 1)) ? 64 : 48) - at * 0.2;
    if (!fuzzy || query.length < 3) return -1;
    var first = name.indexOf(query.charAt(0)), i = 1, j = first + 1;
    if (first < 0) return -1;
    for (; j < name.length && i < query.length; j++) if (name.charAt(j) === query.charAt(i)) i++;
    return i === query.length && j - first <= query.length * 2 ? 16 - (j - first - query.length) : -1;
  }

  function search(text) {
    var query = text.trim().toLowerCase().replace(/\s+/g, "");
    var parentQuery = null;
    var dot = query.lastIndexOf(".");
    if (dot > 0) { parentQuery = query.slice(0, dot); query = query.slice(dot + 1); }
    var found = [];
    function pass(fuzzy) {
      entries.forEach(function (entry) {
        var isObject = entry.kind === "table" || entry.kind === "view" || entry.kind === "procedure";
        if (!text.trim() && !isObject) return;
        if (parentQuery !== null && (isObject || score(entry.parentLower, parentQuery, fuzzy) < 0)) return;
        var s = score(entry.lower, query, false);
        if (s < 0 && fuzzy) s = score(entry.lower, query, true);
        else if (fuzzy) return; // the first pass has it
        if (s < 0) return;
        found.push({ entry: entry, score: s + (isObject ? 12 : 0) });
      });
    }
    pass(false);
    if (!found.length) pass(true);
    found.sort(function (a, b) { return b.score - a.score || a.entry.name.length - b.entry.name.length; });
    var limit = text.trim() ? 60 : 40;
    found = found.slice(0, limit);
    found.sort(function (a, b) {
      return KINDS.indexOf(a.entry.kind) - KINDS.indexOf(b.entry.kind) || b.score - a.score || a.entry.lower.localeCompare(b.entry.lower);
    });
    return { items: found.map(function (f) { return f.entry; }), query: query };
  }

  function highlight(name, query) {
    if (!query) return escapeHtml(name);
    var at = name.toLowerCase().indexOf(query);
    if (at < 0) return escapeHtml(name);
    return escapeHtml(name.slice(0, at)) + "<mark>" + escapeHtml(name.slice(at, at + query.length)) + "</mark>" + escapeHtml(name.slice(at + query.length));
  }

  function render() {
    var text = input.value;
    var result = search(text);
    shown = result.items;
    selected = 0;
    if (!shown.length) {
      results.innerHTML = '<div class="palette-empty"><strong>No results for “' + escapeHtml(text.trim()) + '”</strong>Try a table’s or a column’s name, or table.column.</div>';
      input.removeAttribute("aria-activedescendant");
      return;
    }
    var html = "";
    var group = null;
    shown.forEach(function (entry, i) {
      if (entry.kind !== group) {
        group = entry.kind;
        html += '<div class="palette-group" role="presentation">' + (text.trim() ? GROUPS[group] : "Jump to " + GROUPS[group].toLowerCase()) + "</div>";
      }
      var isMember = entry.kind === "column" || entry.kind === "parameter";
      var name = isMember ? '<span class="palette-parent">' + escapeHtml(entry.parent) + ".</span>" + highlight(entry.name, result.query) : highlight(entry.name, result.query);
      var detail = isMember ? "<code>" + escapeHtml(entry.detail) + "</code>" : escapeHtml(entry.detail);
      html += '<a class="palette-item" role="option" id="search-option-' + i + '" data-index="' + i + '" href="' + escapeHtml(root + entry.url) + '" aria-selected="' + (i === 0) + '">' +
        '<span class="palette-icon is-' + entry.kind + '">' + icon(entry.kind) + "</span>" +
        '<span class="palette-text"><span class="palette-name">' + name + '</span><span class="palette-detail">' + detail + "</span></span>" +
        icon("enter", "icon palette-go") + "</a>";
    });
    results.innerHTML = html;
    results.scrollTop = 0;
    input.setAttribute("aria-activedescendant", "search-option-0");
  }

  function select(index) {
    if (!shown.length) return;
    selected = (index + shown.length) % shown.length;
    $$(".palette-item", results).forEach(function (item) {
      item.setAttribute("aria-selected", item.getAttribute("data-index") === String(selected) ? "true" : "false");
    });
    var current = $("#search-option-" + selected);
    if (current) {
      current.scrollIntoView({ block: "nearest" });
      input.setAttribute("aria-activedescendant", current.id);
    }
  }

  function openSearch() {
    if (!palette || !palette.hidden) return;
    lastFocus = document.activeElement;
    setNav(false);
    palette.hidden = false;
    body.classList.add("palette-open");
    input.value = "";
    render();
    input.focus();
  }
  function closeSearch() {
    if (!palette || palette.hidden) return;
    input.blur(); // a hidden input keeps the focus for a moment otherwise, and takes the next "/"
    palette.hidden = true;
    body.classList.remove("palette-open");
    if (lastFocus && lastFocus !== body && lastFocus.focus) lastFocus.focus({ preventScroll: true });
  }

  if (palette) {
    $$("[data-search-open]").forEach(function (b) { b.addEventListener("click", openSearch); });
    $$("[data-search-close]", palette).forEach(function (b) { b.addEventListener("click", closeSearch); });
    input.addEventListener("input", render);
    input.addEventListener("keydown", function (e) {
      if (e.key === "ArrowDown") { e.preventDefault(); select(selected + 1); }
      else if (e.key === "ArrowUp") { e.preventDefault(); select(selected - 1); }
      else if (e.key === "Enter") {
        e.preventDefault();
        var current = $("#search-option-" + selected);
        if (current) { closeSearch(); window.location.href = current.href; }
      }
    });
    results.addEventListener("mousemove", function (e) {
      var item = e.target.closest(".palette-item");
      if (item && item.getAttribute("data-index") !== String(selected)) select(+item.getAttribute("data-index"));
    });
    results.addEventListener("click", function (e) { if (e.target.closest(".palette-item")) closeSearch(); });
    palette.addEventListener("keydown", function (e) {
      if (e.key === "Tab") { e.preventDefault(); input.focus(); } // the dialog keeps the focus
    });
  }

  document.addEventListener("keydown", function (e) {
    if ((e.key === "k" || e.key === "K") && (e.ctrlKey || e.metaKey)) {
      e.preventDefault();
      if (palette && palette.hidden) openSearch(); else closeSearch();
    } else if (e.key === "/" && !isTyping(e.target) && !e.ctrlKey && !e.metaKey && !e.altKey) {
      e.preventDefault();
      openSearch();
    } else if (e.key === "Escape") {
      if (palette && !palette.hidden) closeSearch();
      else if (body.classList.contains("nav-open")) setNav(false);
    }
  });

  /* On this page: the section in view */

  var tocLinks = $$(".toc a");
  if (tocLinks.length) {
    var targets = tocLinks.map(function (a) { return document.getElementById(decodeURIComponent(a.hash.slice(1))); });
    var ticking = false;
    var spy = function () {
      ticking = false;
      var current = 0;
      targets.forEach(function (target, i) { if (target && target.getBoundingClientRect().top < window.innerHeight * 0.3) current = i; });
      if (window.innerHeight + window.scrollY >= document.documentElement.scrollHeight - 4) current = targets.length - 1;
      tocLinks.forEach(function (a, i) { a.classList.toggle("is-active", i === current); });
    };
    window.addEventListener("scroll", function () { if (!ticking) { ticking = true; requestAnimationFrame(spy); } }, { passive: true });
    spy();
  }

  /* The diagram: hovering a table traces its keys; the database's diagram pans and zooms */

  $$("[data-er]").forEach(function (card) {
    var viewport = $(".er-viewport", card);
    var svg = viewport && $("svg", viewport);
    if (!svg) return;

    var edges = $$(".er-edge", svg);
    var nodes = $$(".er-node", svg);
    function trace(node) {
      var key = node.getAttribute("data-key");
      var near = {};
      edges.forEach(function (edge) {
        var from = edge.getAttribute("data-from"), to = edge.getAttribute("data-to");
        var on = from === key || to === key;
        edge.classList.toggle("is-focus", on);
        if (on) { near[from] = true; near[to] = true; }
      });
      nodes.forEach(function (n) {
        n.classList.toggle("is-focus", n === node);
        n.classList.toggle("is-near", !!near[n.getAttribute("data-key")]);
      });
      svg.classList.add("has-focus");
    }
    function untrace() {
      svg.classList.remove("has-focus");
      $$(".is-focus, .is-near", svg).forEach(function (el) { el.classList.remove("is-focus", "is-near"); });
    }
    nodes.forEach(function (node) {
      node.addEventListener("mouseenter", function () { trace(node); });
      node.addEventListener("mouseleave", untrace);
      node.addEventListener("focus", function () { trace(node); });
      node.addEventListener("blur", untrace);
    });

    if (viewport.classList.contains("er-static")) return;

    var width = +svg.getAttribute("data-width"), height = +svg.getAttribute("data-height");
    var view = { x: 0, y: 0, w: width, h: height };
    var zoomLabel = $(".er-zoom", card);
    svg.removeAttribute("width");
    svg.removeAttribute("height");
    svg.setAttribute("preserveAspectRatio", "xMidYMid meet");
    viewport.classList.add("is-interactive");

    function size() { return { w: viewport.clientWidth || 1, h: viewport.clientHeight || 1 }; }
    function apply() {
      var s = size();
      view.h = view.w * s.h / s.w;
      svg.setAttribute("viewBox", [view.x, view.y, view.w, view.h].map(function (v) { return Math.round(v * 10) / 10; }).join(" "));
      if (zoomLabel) zoomLabel.textContent = Math.round(s.w / view.w * 100) + "%";
    }
    // The viewport's height: the whole diagram at the page's width, up to most of the window.
    function fitHeight() {
      if (card.classList.contains("is-full")) { viewport.style.height = ""; return; }
      var s = size();
      var natural = s.w * height / width;
      var max = Math.max(440, window.innerHeight * 0.78);
      viewport.style.height = Math.round(Math.min(Math.max(natural, 280), max)) + "px";
    }
    function fit() {
      var s = size();
      var scale = Math.min(s.w / width, s.h / height);
      view.w = s.w / scale;
      view.x = (width - view.w) / 2;
      view.h = s.h / scale;
      view.y = (height - view.h) / 2;
      apply();
    }
    // The first view: the whole diagram (never above 100%); a large one at half size or more, from its top left.
    function initial() {
      fitHeight();
      var s = size();
      var fitScale = Math.min(1, s.w / width, s.h / height);
      var scale = Math.max(fitScale, Math.min(0.5, s.w / width));
      view.w = s.w / scale;
      if (scale <= fitScale + 0.001) {
        view.x = (width - view.w) / 2;
        view.y = (height - s.h / scale) / 2;
      } else {
        view.x = Math.min(0, (width - view.w) / 2);
        view.y = 0;
      }
      apply();
    }
    function zoom(factor, cx, cy) {
      var s = size();
      if (cx === undefined) { cx = s.w / 2; cy = s.h / 2; }
      var px = view.x + cx / s.w * view.w, py = view.y + cy / s.h * view.h;
      var widest = Math.max(width, height * s.w / s.h) * 1.6, narrowest = Math.max(s.w / 3, 120); // 33% to 300%
      var w = Math.min(widest, Math.max(narrowest, view.w / factor));
      view.x = px - cx / s.w * w;
      view.y = py - cy / s.h * (w * s.h / s.w);
      view.w = w;
      apply();
    }

    $$("[data-er-zoom]", card).forEach(function (button) {
      button.addEventListener("click", function () {
        var what = button.getAttribute("data-er-zoom");
        if (what === "in") zoom(1.25); else if (what === "out") zoom(0.8); else fit();
      });
    });
    var full = $("[data-er-full]", card);
    function setFull(on) {
      var s = size(), cx = view.x + view.w / 2, cy = view.y + view.h / 2, scale = s.w / view.w;
      card.classList.toggle("is-full", on);
      body.classList.toggle("er-full", on);
      if (full) full.setAttribute("aria-label", on ? "Exit full screen" : "Full screen");
      fitHeight();
      s = size();
      view.w = s.w / scale;
      view.x = cx - view.w / 2;
      view.y = cy - (view.w * s.h / s.w) / 2;
      apply();
    }
    if (full) full.addEventListener("click", function () { setFull(!card.classList.contains("is-full")); });
    document.addEventListener("keydown", function (e) { if (e.key === "Escape" && card.classList.contains("is-full")) setFull(false); });

    viewport.addEventListener("wheel", function (e) {
      var rect = viewport.getBoundingClientRect();
      if (e.ctrlKey || e.metaKey) {
        e.preventDefault();
        zoom(Math.exp(-e.deltaY * (e.deltaMode === 1 ? 0.05 : 0.0022)), e.clientX - rect.left, e.clientY - rect.top);
      } else if (card.classList.contains("is-full")) {
        e.preventDefault();
        var s = size();
        view.x += e.deltaX * view.w / s.w;
        view.y += e.deltaY * view.w / s.w;
        apply();
      }
    }, { passive: false });

    var drag = null, dragged = false;
    viewport.addEventListener("pointerdown", function (e) {
      if (e.button !== 0) return;
      if (e.pointerType === "mouse") e.preventDefault(); // not the browser's own drag of a table's link
      drag = { x: e.clientX, y: e.clientY, vx: view.x, vy: view.y, id: e.pointerId, moved: false };
      dragged = false;
    });
    viewport.addEventListener("dragstart", function (e) { e.preventDefault(); });
    viewport.addEventListener("pointermove", function (e) {
      if (!drag || e.pointerId !== drag.id) return;
      var dx = e.clientX - drag.x, dy = e.clientY - drag.y;
      if (!drag.moved && Math.abs(dx) + Math.abs(dy) < 4) return;
      if (!drag.moved) {
        drag.moved = true;
        viewport.classList.add("is-dragging");
        try { viewport.setPointerCapture(e.pointerId); } catch (err) { /* already released */ }
      }
      var s = size();
      view.x = drag.vx - dx * view.w / s.w;
      view.y = drag.vy - dy * view.w / s.w;
      apply();
    });
    function endDrag() {
      if (drag && drag.moved) dragged = true;
      drag = null;
      viewport.classList.remove("is-dragging");
    }
    viewport.addEventListener("pointerup", endDrag);
    viewport.addEventListener("pointercancel", endDrag);
    viewport.addEventListener("click", function (e) {
      if (dragged) { e.preventDefault(); e.stopPropagation(); dragged = false; }
    }, true);

    viewport.addEventListener("keydown", function (e) {
      var s = size(), step = 60 * view.w / s.w;
      if (e.key === "+" || e.key === "=") zoom(1.25);
      else if (e.key === "-" || e.key === "_") zoom(0.8);
      else if (e.key === "0") fit();
      else if (e.key === "ArrowLeft") { view.x -= step; apply(); }
      else if (e.key === "ArrowRight") { view.x += step; apply(); }
      else if (e.key === "ArrowUp") { view.y -= step; apply(); }
      else if (e.key === "ArrowDown") { view.y += step; apply(); }
      else return;
      e.preventDefault();
    });

    var lastWidth = 0;
    function resized() {
      var w = viewport.clientWidth;
      if (!w || w === lastWidth) return;
      var first = lastWidth === 0;
      var scale = first ? 0 : lastWidth / view.w;
      var cx = view.x + view.w / 2;
      lastWidth = w;
      if (first) { initial(); return; }
      fitHeight();
      view.w = w / scale;
      view.x = cx - view.w / 2;
      apply();
    }
    if (window.ResizeObserver) new ResizeObserver(resized).observe(viewport);
    else window.addEventListener("resize", resized);
    resized();
  });
})();
