import React from "react";
import "./Sidebar.scss";
import SidebarOption from "./sidebarOption/SidebarOption";

class Sidebar extends React.Component {
    render() {
        return (
            <div className="sidebar pageHoverElement">
                <SidebarOption MainContent={1} Url={"ur"}/>
                <SidebarOption MainContent={2} OpenContent={"Super Text!!"} Url={"q"}/>
                <SidebarOption MainContent={3} Url={"/"}/>
                <SidebarOption MainContent={4}/>
                <SidebarOption MainContent={5}/>
            </div>
        );
    }
}

export default Sidebar;