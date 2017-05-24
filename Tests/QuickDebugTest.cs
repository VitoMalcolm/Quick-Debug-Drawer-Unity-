using System;
using DebugUtils;
using UnityEngine;

public class QuickDebugTest : MonoBehaviour
{
    IDebugDrawer dbg = new QuickDebugDrawer() { sx = 50f, sy = 50f };

    void OnGUI()
    {
        dbg.Prepare();
        dbg.ShowInfo(
        "@@System Info|400", //width = 400 (optional)

                "Current date", DateTime.Now,
                "OS (family)", String.Format("{0} ({1})", SystemInfo.operatingSystem, SystemInfo.operatingSystemFamily),

            "**Device",
                "Name", SystemInfo.deviceName,
                "Type", SystemInfo.deviceType,
                "Model", SystemInfo.deviceModel,
                "ID", SystemInfo.deviceUniqueIdentifier,

            "**CPU",
                "Type", SystemInfo.processorType,
                "Count/threads", SystemInfo.processorCount,
                "Frequency", SystemInfo.processorFrequency,

            "**GPU",
                "Name", SystemInfo.graphicsDeviceName,
                "Version", SystemInfo.graphicsDeviceVersion,
                "Type", SystemInfo.graphicsDeviceType,
                "---", //separator
                "Memory size", SystemInfo.graphicsMemorySize,
                "Shader level", SystemInfo.graphicsShaderLevel,
                "ID:", SystemInfo.graphicsDeviceID,

        "@@System support",

            "**Render targets",
                "Max targets", SystemInfo.supportedRenderTargetCount,
                "Render 3D textures", SystemInfo.supports3DRenderTextures,
                "Render to cubemap", SystemInfo.supportsRenderToCubemap,
                "Render HDR textures", SystemInfo.SupportsRenderTextureFormat(
                        RenderTextureFormat.ARGBHalf),
                "Image effects", SystemInfo.supportsImageEffects,

            "**Advanced",
                "Compute shaders", SystemInfo.supportsComputeShaders,
                "Instanced rendering", SystemInfo.supportsInstancing,
                "Motion vectors", SystemInfo.supportsMotionVectors,
                "2D array textures", SystemInfo.supports2DArrayTextures,
                "3D textures", SystemInfo.supports3DTextures,
                "Cubemap arrays", SystemInfo.supportsCubemapArrayTextures
        );
    }
}
