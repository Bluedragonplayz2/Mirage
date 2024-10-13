import Image from "next/image";

export default function Home() {
  return (
    <div className={"w-screen h-screen flex"}>
        <div className={"w-1/4 flex flex-col  " }>
            <div id={"robotMenu"} className={"w-full h-4/6 bg-amber-200"}> robot list</div>

        </div>

        <div className={"w-3/4 flex flex-col "} >
            <div id={"mapView"} className={"w-full h-4/6 bg-amber-300"}>main map</div>
            <div id={"editor"} className={"w-full h-2/6 bg-amber-400"}></div>
        </div>


    </div>

  );
}
