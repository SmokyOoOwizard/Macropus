import React from "react";
import "./PageTopbarWrapper.scss";
import Topbar from "../topbar/Topbar";

class IProps {
    children?: React.ReactNode
}

type Props = IProps;

class PageTopbarWrapper extends React.Component<Props> {

    render() {
        return <div className="pageTopbarWrapper">
            <div>
                {
                    this.props.children
                }
            </div>
            {
                <div className="content">
                    <Topbar/>
                </div>
            }
        </div>
    }
}

export default PageTopbarWrapper;