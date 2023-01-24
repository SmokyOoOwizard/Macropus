import React from 'react';
import {connect} from "react-redux";
import {SetApiToken} from "../../app/AppSlice";

interface SliceProps {
    Token?: string
}

interface IDispatch {
    SetApiToken: (apiToken?: string) => void;
}

interface IProps {
    LoginPage: React.ReactNode,
    children?: React.ReactNode
}

type Props = SliceProps & IProps & IDispatch;

class LoginChecker extends React.Component<Props> {

    constructor(props: Props) {
        super(props);
    }

    render() {
        return this.props.Token != undefined ? this.props.children : this.props.LoginPage
    }
}

const mapStateToProps = (store: any) => {
    return {
        Token: store.app.apiToken
    }
}

const mapDispatchToProps = (dispatch: any) => {
    return {
        SetApiToken: (apiToken?: string) => {
            dispatch(SetApiToken(apiToken))
        }
    }
}

export default connect<SliceProps, IDispatch, IProps>(mapStateToProps, mapDispatchToProps)(LoginChecker)
