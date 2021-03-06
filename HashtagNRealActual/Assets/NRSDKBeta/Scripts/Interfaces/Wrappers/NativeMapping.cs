﻿/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /**
    * @brief Session Native API.
    */
    internal partial class NativeMapping
    {
        private UInt64 m_DatabaseHandle;

        private static NativeInterface m_NativeInterface;

        public NativeMapping(NativeInterface nativeInterface)
        {
            m_NativeInterface = nativeInterface;
        }

        public bool CreateDataBase()
        {
            var result = NativeApi.NRWorldMapDatabaseCreate(m_NativeInterface.TrackingHandle, ref m_DatabaseHandle);
            NRDebugger.Log("[NativeMapping] End to create worldmap :" + result);
            return result == NativeResult.Success;
        }

        public bool DestroyDataBase()
        {
            var result = NativeApi.NRWorldMapDatabaseDestroy(m_NativeInterface.TrackingHandle, m_DatabaseHandle);
            NRDebugger.Log("[NativeMapping] End to destroy worldmap :" + result);
            return result == NativeResult.Success;
        }

        public bool LoadMap(string path)
        {
            NRDebugger.Log("[NativeMapping] Start to load worldmap:" + path);
            var result = NativeApi.NRWorldMapDatabaseLoadFile(m_NativeInterface.TrackingHandle, m_DatabaseHandle, path);
            NRDebugger.Log("[NativeMapping] End to load worldmap :" + result);
            return result == NativeResult.Success;
        }

        public bool SaveMap(string path)
        {
            var result = NativeApi.NRWorldMapDatabaseSaveFile(m_NativeInterface.TrackingHandle, m_DatabaseHandle, path);
            NRDebugger.Log("[NativeMapping] Save worldmap :" + result);
            return result == NativeResult.Success;
        }

        public UInt64 AddAnchor(Pose pose)
        {
            UInt64 anchorHandle = 0;
            NativeMat4f nativePose;
            ConversionUtility.UnityPoseToApiPose(pose, out nativePose);
            var result = NativeApi.NRTrackingAcquireNewAnchor(m_NativeInterface.TrackingHandle, ref nativePose, ref anchorHandle);
            NRDebugger.Log("[NativeMapping] add anchor :" + result);
            return anchorHandle;
        }

        public UInt64 CreateAnchorList()
        {
            UInt64 anchorlisthandle = 0;
            var result = NativeApi.NRAnchorListCreate(m_NativeInterface.TrackingHandle, ref anchorlisthandle);
            NRDebugger.Log("[NativeMapping] Creat anchor list:" + anchorlisthandle);
            return anchorlisthandle;
        }

        public bool UpdateAnchor(UInt64 anchorlisthandle)
        {
            var result = NativeApi.NRTrackingUpdateAnchors(m_NativeInterface.TrackingHandle, anchorlisthandle);
            //Debug.Log("UpdateAnchor :" + result);
            NRDebugger.Log("[NativeMapping] Update anchor list:" + anchorlisthandle);
            return result == NativeResult.Success;
        }

        public bool DestroyAnchorList(UInt64 anchorlisthandle)
        {
            //NRDebugger.Log("Start to destroy anchor list...");
            var result = NativeApi.NRAnchorListDestroy(m_NativeInterface.TrackingHandle, anchorlisthandle);
            //NRDebugger.Log("End to destroy anchor :" + result);
            NRDebugger.Log("[NativeMapping] Destroy anchor list:" + anchorlisthandle);
            return result == NativeResult.Success;
        }

        public int GetAnchorListSize(UInt64 anchor_list_handle)
        {
            int size = 0;
            NativeApi.NRAnchorListGetSize(m_NativeInterface.TrackingHandle, anchor_list_handle, ref size);
            NRDebugger.Log("[NativeMapping] Get anchor list size:" + size);
            return size;
        }

        public UInt64 AcquireItem(UInt64 anchor_list_handle, int index)
        {
            UInt64 anchorHandle = 0;
            NativeApi.NRAnchorListAcquireItem(m_NativeInterface.TrackingHandle, anchor_list_handle, index, ref anchorHandle);
            NRDebugger.Log("[NativeMapping] Acquire anchor item:" + anchorHandle);
            return anchorHandle;
        }

        public TrackingState GetTrackingState(UInt64 anchor_handle)
        {
            TrackingState trackingState = TrackingState.Stopped;
            NativeApi.NRAnchorGetTrackingState(m_NativeInterface.TrackingHandle, anchor_handle, ref trackingState);
            NRDebugger.Log("[NativeMapping] Creat tracking state:" + trackingState);
            return trackingState;
        }

        public static int GetAnchorNativeID(UInt64 anchor_handle)
        {
            int anchorID = -1;
            NativeApi.NRAnchorGetID(m_NativeInterface.TrackingHandle, anchor_handle, ref anchorID);
            NRDebugger.Log("[NativeMapping] Get anchor id:" + anchorID);
            return anchorID;
        }

        public Pose GetAnchorPose(UInt64 anchor_handle)
        {
            NativeMat4f nativePose = NativeMat4f.identity;
            NativeApi.NRAnchorGetPose(m_NativeInterface.TrackingHandle, anchor_handle, ref nativePose);

            Pose unitypose;
            ConversionUtility.ApiPoseToUnityPose(nativePose, out unitypose);
            //Debug.Log("nativePose : " + nativePose.ToString() + " unitypose:" + unitypose);
            NRDebugger.Log("[NativeMapping] Get anchor pose:" + unitypose.ToString());
            return unitypose;
        }

        public bool DestroyAnchor(UInt64 anchor_handle)
        {
            var result = NativeApi.NRAnchorDestroy(m_NativeInterface.TrackingHandle, anchor_handle);
            NRDebugger.Log("[NativeMapping] Destroy anchor:" + result);
            return result == NativeResult.Success;
        }

        private partial struct NativeApi
        {
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRWorldMapDatabaseCreate(UInt64 tracking_handle,
                ref UInt64 out_world_map_database_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRWorldMapDatabaseDestroy(UInt64 tracking_handle,
                UInt64 world_map_database_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRWorldMapDatabaseLoadFile(UInt64 tracking_handle,
                UInt64 world_map_database_handle, string world_map_database_file_path);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRWorldMapDatabaseSaveFile(UInt64 tracking_handle,
                UInt64 world_map_database_handle, string world_map_database_file_path);

            // NRTracking
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingAcquireNewAnchor(
                 UInt64 tracking_handle, ref NativeMat4f pose, ref UInt64 out_anchor_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingUpdateAnchors(
               UInt64 tracking_handle, UInt64 out_anchor_list_handle);

            // NRAnchorList
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorListCreate(
                 UInt64 tracking_handle, ref UInt64 out_anchor_list_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorListDestroy(
                 UInt64 tracking_handle, UInt64 anchor_list_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorListGetSize(UInt64 tracking_handle,
                UInt64 anchor_list_handle, ref int out_list_size);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorListAcquireItem(UInt64 tracking_handle,
                UInt64 anchor_list_handle, int index, ref UInt64 out_anchor);

            // NRAnchor
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorGetTrackingState(UInt64 tracking_handle,
                UInt64 anchor_handle, ref TrackingState out_tracking_state);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorGetID(UInt64 tracking_handle,
                UInt64 anchor_handle, ref int out_anchor_id);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorGetPose(UInt64 tracking_handle,
                UInt64 anchor_handle, ref NativeMat4f out_pose);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorDestroy(UInt64 tracking_handle,
                UInt64 anchor_handle);
        }
    }
}
