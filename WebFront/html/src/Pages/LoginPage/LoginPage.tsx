import React from "react";
import "../Page.scss";
import "./LoginPage.scss";
import {SetApiToken} from "../../app/AppSlice";
import {connect} from "react-redux";

interface IDispatch {
    SetApiToken: (apiToken?: string) => void;
}

type Props = IDispatch;

class LoginPage extends React.Component<Props> {

    constructor(props: Props) {
        super(props);

        this.OnLogin = this.OnLogin.bind(this);
    }

    OnLogin() {
        console.log("LOGIN");

        // TODO some async magick to get token

        this.props.SetApiToken("TOKEN");
    }

    render() {
        return <div className="page">
            <div className="loginPage">
                <h1>Macropus</h1>
                <input type="text" placeholder="username"/>
                <input type="password" placeholder="password"/>

                <div className="login-btn" onClick={this.OnLogin}>
                    Login
                </div>
            </div>
        </div>;
    }
}

const mapDispatchToProps = (dispatch: any) => {
    return {
        SetApiToken: (apiToken?: string) => {
            dispatch(SetApiToken(apiToken))
        }
    }
}

export default connect<void, IDispatch>(null, mapDispatchToProps)(LoginPage)