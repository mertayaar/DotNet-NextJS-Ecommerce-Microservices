





(function (global) {
    const SignalRClient = {
        _connection: null,

        buildUrl() {
            
            return window.location.origin.replace(/:\d+$/, '') + ':5165' + '/ecommercehub';
        },

        getToken() {
            
            return localStorage.getItem('jwtToken');
        },

        on(methodName, handler) {
            if (this._connection) this._connection.on(methodName, handler);
        },

        off(methodName, handler) {
            if (this._connection) this._connection.off(methodName, handler);
        },

        async connect() {
            if (this._connection && this._connection.state === signalR.HubConnectionState.Connected) {
                return this._connection;
            }

            const tokenFactory = () => this.getToken();

            const hubUrl = this.buildUrl();

            this._connection = new signalR.HubConnectionBuilder()
                .withUrl(hubUrl, { accessTokenFactory: tokenFactory })
                .withAutomaticReconnect()
                .build();

            this._connection.onreconnecting(error => console.warn('SignalR reconnecting', error));
            this._connection.onreconnected(connectionId => console.info('SignalR reconnected', connectionId));
            this._connection.onclose(error => console.warn('SignalR closed', error));

            try {
                await this._connection.start();
                console.info('SignalR connected');
            } catch (err) {
                console.error('SignalR connection error', err);
                throw err;
            }

            return this._connection;
        },

        async invoke(methodName, ...args) {
            if (!this._connection) await this.connect();
            return this._connection.invoke(methodName, ...args);
        }
    };

    global.SignalRClient = SignalRClient;
})(window);
