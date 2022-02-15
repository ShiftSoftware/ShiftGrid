const template = document.createElement('template');
template.innerHTML = `
  <div class="card">
    <div class="card-body"></div>
  </div>
`



class ShiftGrid extends HTMLElement {
    constructor() {
        super();
        this._shadowRoot = this.attachShadow({ 'mode': 'open' });

        if (this.hasAttribute('theme')) {
            const linkElem = document.createElement('link');
            linkElem.setAttribute('rel', 'stylesheet');
            linkElem.setAttribute('href', this.getAttribute('theme'));


            this._shadowRoot.appendChild(linkElem);
        }

        this._shadowRoot.appendChild(template.content.cloneNode(true));
    }

    get longitude() {
        return this.getAttribute('longitude');
    }

    get latitude() {
        return this.getAttribute('latitude');
    }

    connectedCallback() {
        this.$card = this._shadowRoot.querySelector('.card-body');

        let $townName = document.createElement('p');
        $townName.innerHTML = `Lat: ${this.latitude}`;
        this.$card.appendChild($townName);

        let $temperature = document.createElement('p');
        $temperature.innerHTML = `Long: ${this.longitude}`
        this.$card.appendChild($temperature);
    }
}

customElements.define('shift-grid', ShiftGrid);
