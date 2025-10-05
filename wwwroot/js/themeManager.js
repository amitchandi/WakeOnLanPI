window.themeManager = {
    key: "theme",

    apply(theme) {
        document.documentElement.setAttribute("data-theme", theme);
        localStorage.setItem(this.key, theme);
    },

    getSystemPreference() {
        return window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
    },

    init() {
        let theme = localStorage.getItem(this.key);
        if (!theme) theme = this.getSystemPreference();
        this.apply(theme);
        return theme === "dark";
    },

    toggle() {
        const current = document.documentElement.getAttribute("data-theme");
        const next = current === "dark" ? "light" : "dark";
        this.apply(next);
        return next === "dark";
    }
};
