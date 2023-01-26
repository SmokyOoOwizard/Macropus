import React from "react";
import Sidebar from "../sidebar/Sidebar";
import "./PageSidebarWrapper.scss";

class IProps {
    children?: React.ReactNode
    barEnable?: boolean = false;
}

type Props = IProps;

class PageSidebarWrapper extends React.Component<Props> {

    render() {
        return <div className="pageSidebarWrapper">
            <div>
                {
                    this.props.children
                }
            </div>
            {
                this.props.barEnable ? <Sidebar/> : undefined
            }
        </div>
    }
}

export default PageSidebarWrapper;