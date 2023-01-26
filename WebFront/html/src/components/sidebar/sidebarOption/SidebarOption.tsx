import React from "react";
import "./SidebarOption.scss";

interface IProps {
    MainContent: React.ReactNode
    OpenContent?: React.ReactNode,
    Url?: string
}

type Props = IProps;

class SidebarOption extends React.Component<Props> {

    getOpenContent() {
        if (this.props.OpenContent == undefined)
            return undefined;

        return (
            <div className="openContent">
                <div>
                    {
                        this.props.OpenContent
                    }
                </div>
            </div>
        );
    }

    render() {
        return (
            <a className="sidebarOption" href={this.props.Url}>
                <div className="sidebarOptionTongue">
                    <div className="mainContent">
                        {
                            this.props.MainContent
                        }
                    </div>
                    {
                        this.getOpenContent()
                    }
                </div>
            </a>
        );
    }
}

export default SidebarOption;
