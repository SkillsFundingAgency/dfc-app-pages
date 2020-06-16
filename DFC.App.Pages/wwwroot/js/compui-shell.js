class CompUiShell {
    constructor(allChildApps) {
        this.allChildApps = allChildApps;

        this.validation = new CompUiValidation();
    }

    initialise() {
        this.validation.initialise();
        this.allChildApps.forEach(f => f.initialise());
    }
}

var compUiShell = new CompUiShell(
    [
        new DfcAppPages()
    ]);

window.onload = compUiShell.initialise();
